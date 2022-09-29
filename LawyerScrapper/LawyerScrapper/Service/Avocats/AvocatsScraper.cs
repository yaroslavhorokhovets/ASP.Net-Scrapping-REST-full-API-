using HtmlAgilityPack;
using LawyerScrapper.Data;
using LawyerScrapper.Helper;
using LawyerScrapper.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public class AvocatsScraper : IAvocatsScraper
    {
        private RestClient _client;
        public AvocatsScraper() { 
            _client = new RestClient("https://www.avocats.be/");
        }

        public void Dispose()
        {
            
        }

        public async Task<LawyerDto> GetLawyerDetail(string fullName)
        {
            var request = new RestRequest("lawyersapisearch");
            request.AddParameter("search", fullName);
            request.AddParameter("page", "1");
            request.AddParameter("size", "10");
            var response = await _client.PostAsync(request);
            var results = JsonConvert.DeserializeObject<AvocatsResults>(response.Content);
            int pages = results.Pages;
            if (pages > 0)
            {
                var dto = results.Results.Where(x => Utils.RemoveDutchSpecialChars(x.Name).ToLower() == Utils.RemoveDutchSpecialChars(fullName).ToLower()).FirstOrDefault();
                if (dto != null)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(dto.LawyerInfoSection);

                    List<string> addressList = new List<string>();
                    List<string> phoneList = new List<string>();
                    List<string> faxList = new List<string>();
                    List<string> emailList = new List<string>();
                    List<string> websiteList = new List<string>();

                    var aside = doc.DocumentNode.SelectSingleNode("//*[@id='info-section-content']");
                    var addressDivs = aside.SelectNodes(".//ul[contains(@class, 'cabinet-address')]");
                    foreach (var ul in addressDivs)
                    {
                        addressList.Add(Utils.CleanChars(ul.InnerText.Replace("Ouvrir map", "")));
                    }
                    var phoneUl = aside.SelectSingleNode(".//ul[contains(@class, 'cellphone')]");
                    string phoneString = "";
                    if (phoneUl != null)
                    {
                        phoneString = phoneUl.InnerText;
                    }

                    var phoneMatches = Regex.Matches(phoneString, @"phone\+([0-9\s]+)");
                    foreach (Match m in phoneMatches)
                    {
                        phoneList.Add(Utils.CleanChars(m.Value.ToString().Replace("phone+", "Tel. ")));
                    }
                    phoneMatches = Regex.Matches(phoneString, @"mobile\+([0-9\s]+)");
                    foreach (Match m in phoneMatches)
                    {
                        phoneList.Add(Utils.CleanChars(m.Value.ToString().Replace("mobile+", "Tel. ")));
                    }
                    phoneMatches = Regex.Matches(phoneString, @"fax\+([0-9\s]+)");
                    foreach (Match m in phoneMatches)
                    {
                        faxList.Add(Utils.CleanChars(m.Value.ToString().Replace("fax+", "Fax. ")));
                    }
                    var siteUl = aside.SelectSingleNode(".//ul[contains(@class, 'site')]");
                    string siteString = "";
                    if (siteUl != null)
                    {
                        siteString = siteUl.InnerText.Replace("e-mail", "");
                    }
                    var siteMatches = Regex.Matches(siteString, @"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)");
                    foreach (Match m in siteMatches)
                    {
                        emailList.Add(Utils.CleanChars(m.Value.ToString().Replace("phone+", "Tel. ")));
                    }
                    var firstName = fullName.Substring(0, fullName.LastIndexOf(" "));
                    var lastName = fullName.Substring(fullName.LastIndexOf(" ") + 1);
                    return new LawyerDto
                    {
                        Name = firstName,
                        SureName = lastName,
                        Phone = phoneList.Count > 0 ? $"[\"{string.Join("\", \"", phoneList)}\"]" : "[]",
                        Fax = faxList.Count > 0 ? $"[\"{string.Join("\", \"", faxList)}\"]" : "[]",
                        Email = emailList.Count > 0 ? $"[\"{string.Join("\", \"", emailList)}\"]" : "[]",
                        Website = websiteList.Count > 0 ? $"[\"{string.Join("\", \"", websiteList)}\"]" : "[]",
                        Address = addressList.Count > 0 ? $"[\"{string.Join("\", \"", addressList)}\"]" : "[]",
                    };
                }
            }
            
            return null;
        }

    }
}
