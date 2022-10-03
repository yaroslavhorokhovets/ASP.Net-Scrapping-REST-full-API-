using LawyerScrapper.Data;
using LawyerScrapper.Helper;
using LawyerScrapper.Model;
using LawyerScrapper.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace LawyerScrapper
{
    internal class Program
    {
        private static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            string searchStringValue = "REC";
            Console.WriteLine("Scrapper started");
            RegisterServices();
            string[] langs = { "nl", "fr" };
            ScrapeCourtList(searchStringValue, langs[0]).Wait();
        }
        public static void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddDbContext<LawyerDbContext>(options => options.UseSqlServer(ConnectionString));
            collection.AddScoped<ILawyerDbService, LawyerDbService>();
            collection.AddScoped<IDossierScraper, DossierScraper>();
            collection.AddScoped<IAdvocaatScraper, AdvocaatScraper>();
            collection.AddScoped<IAvocatsScraper, AvocatsScraper>();
            _serviceProvider = collection.BuildServiceProvider();

            // create Database if does not exist
            var context = _serviceProvider.GetService<LawyerDbContext>();
            context.Database.EnsureCreated();
        }

        public static async Task ScrapeCourtList(string searchString, string lg)
        {
            var dossierScraper = _serviceProvider.GetService<IDossierScraper>();
            var dbService = _serviceProvider.GetService<ILawyerDbService>();
            var courtList = await dossierScraper.GetCourtList(searchString, lg);
            Console.WriteLine($"The number of courts is {courtList.Count} in the Belgium.");
            var result = await dbService.InsertCourtData(courtList, lg);
            Console.WriteLine(result);
            foreach(var court in courtList)
            {
                court.AvailableDates = await dossierScraper.GetAvailableDates(court, lg);
            }

            foreach (var court in courtList)
            {
                if(court.AvailableDates != null)
                {
                    if (court.AvailableDates.Count <= 0)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var date in court.AvailableDates)
                        {
                            if (date != Constants.today)
                            {
                                continue;
                            }
                            ScrapeCourtDetailList(court, date, lg).Wait();
                        }
                    }
                }
                
            }
        }

        public static async Task ScrapeCourtDetailList(CourtDto courtDto, string availableDate, string lg)
        {
            var dossierScraper = _serviceProvider.GetService<IDossierScraper>();
            var dbService = _serviceProvider.GetService<ILawyerDbService>();
            var courtcaseList = await dossierScraper.GetSpecificDateCourtCaseList(courtDto, availableDate, lg);
            Console.WriteLine($"The number of the court cases is {courtcaseList.Count} at {availableDate}.");
            var courtWithNames = new List<CourtDetailDto>();


            foreach (var court in courtcaseList)
            {
                if (!string.IsNullOrEmpty(court.LaywersLink))
                {
                    var courtWithName = await dossierScraper.GetLawyerNamesFromCourt(court);
                    if (courtWithName.Lawyers.Count > 0)
                    {
                        courtWithNames.Add(courtWithName);
                    }
                }
            }
            
            if (courtWithNames.Count == 0)
            {
                Console.WriteLine($"There are no court cases with lawyers' names.");
                return;
            }

            List<string> fullNames = await dbService.InsertCourtCasesData(courtWithNames, lg);
            Console.WriteLine($"You have successfully inserted court case!");
            InsertLawyerDetail(fullNames, lg).Wait();
            return;
        }

        public static async Task InsertLawyerDetail(List<string> fullNames, string lg)
        {
            var advocaatScraper = _serviceProvider.GetService<IAdvocaatScraper>();
            var dbService = _serviceProvider.GetService<ILawyerDbService>();
            var abocatsScraper = _serviceProvider.GetService<IAvocatsScraper>();

            List<LawyerDto> lawyers = new List<LawyerDto>();

            List<string> notFoundList = new List<string>();
            List<string> notFoundFinalList = new List<string>();

            // scrape option 1 https://advocaat.be/
            List<string> distinctedfullNames = fullNames.Distinct().ToList();

            foreach (var fullName in distinctedfullNames)
            {
                var lawyerDetail = await advocaatScraper.GetLawyerDetail(fullName);
                if (lawyerDetail != null)
                {
                    lawyers.Add(lawyerDetail);
                }
                else
                {
                    notFoundList.Add(fullName);
                }
            }
            if (lawyers.Count > 0)
            {
                // insert lawyer data to lawyer table
                string result = await dbService.InsertLawyerData(lawyers, lg);
                Console.WriteLine($"{result}");
                lawyers.Clear();
            }
            // scrape option 2 https://www.avocats.be/node/18
            foreach (var fullName in notFoundList)
            {
                var lawyerDetail = await abocatsScraper.GetLawyerDetail(fullName);
                if (lawyerDetail != null)
                {
                    lawyers.Add(lawyerDetail);
                }
                else
                {
                    notFoundFinalList.Add(fullName);
                }
            }
            if (lawyers.Count > 0)
            {
                // insert lawyer data to lawyer table
                string result = await dbService.InsertLawyerData(lawyers, lg);
                Console.WriteLine($"{result}");
                lawyers.Clear();
            }

            Console.WriteLine($"Not found {notFoundFinalList.Count} Lawyers {string.Join(", ", notFoundFinalList)}");
            Console.WriteLine($"Total: {fullNames.Count} Lawyers");
        }

    }
}
