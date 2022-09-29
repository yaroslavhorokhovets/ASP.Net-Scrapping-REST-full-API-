using HtmlAgilityPack;
using LawyerScrapper.Data;
using LawyerScrapper.Helper;
using LawyerScrapper.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public class AdvocaatScraper : IAdvocaatScraper
    {
        private RestClient _client;
        public AdvocaatScraper() { 
            _client = new RestClient("https://advocaat.be");
        }

        public void Dispose()
        {
            
        }

        public async Task<LawyerDto> GetLawyerDetail(string fullName)
        {
            var request = new RestRequest("zoek-een-advocaat/zoekresultaten");
            request.AddParameter("mtm_campaign", "Search-Zoeken-Algemeen");
            request.AddParameter("mtm_source", "google");
            request.AddParameter("mtm_medium", "cpc");
            request.AddParameter("gclid", "CjwKCAjwvNaYBhA3EiwACgndgtJYF4bQw7RlOjx55_z86UOTPJVVcGQItebKhA8Y7Q7T503WjL1sMhoCcGkQAvD_BwE");
            request.AddParameter("n", fullName);
            var response = await _client.GetAsync(request);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var divSearchResults = doc.DocumentNode.SelectNodes("//div[contains(@class, 'card card--compact search-result')]");
            string detailLink = "";
            string firstName = "";
            string lastName = "";
            if (divSearchResults != null && divSearchResults.Count > 0)
            {
                foreach (var div in divSearchResults)
                {
                    var h2Tag = div.SelectSingleNode(".//h2[contains(@class, 'card__title search-result__title')]");
                    firstName = h2Tag.SelectSingleNode(".//strong").InnerText;
                    string scrapedFullName = h2Tag.InnerText;
                    scrapedFullName = scrapedFullName.Replace("\n", "").Replace("\r", "").Replace("  ", "");
                    if (Utils.RemoveDutchSpecialChars(scrapedFullName).ToLower() == Utils.RemoveDutchSpecialChars(fullName).ToLower())
                    {
                        detailLink = h2Tag.SelectSingleNode(".//a").Attributes["href"].Value;
                        lastName = scrapedFullName.Replace(firstName + " ", "");
                    }
                }
            }
            
            List<string> addressList = new List<string>();
            List<string> phoneList = new List<string>();
            List<string> faxList = new List<string>();
            List<string> emailList = new List<string>();
            List<string> websiteList = new List<string>();

            if (!string.IsNullOrEmpty(detailLink))
            {
                request = new RestRequest(detailLink);
                response = await _client.GetAsync(request);
                doc.LoadHtml(response.Content);
                var aside = doc.DocumentNode.SelectSingleNode("//aside[contains(@class, 'col-md-4 detail-view__aside')]");
                
                if (aside != null)
                {
                    var contactDiv = aside.SelectSingleNode(".//div");
                    if (contactDiv != null)
                    {
                        
                        var divs = contactDiv.SelectNodes(".//div");
                        if (divs != null && divs.Count >= 3)
                        {
                            addressList.Add(divs[0].InnerText + ", " + divs[1].InnerText);
                            
                            var hrNext = contactDiv.SelectSingleNode(".//hr").NextSibling;
                            if (hrNext != null)
                            {
                                hrNext = hrNext.NextSibling;
                                if (hrNext != null && hrNext.Name == "div")
                                {
                                    addressList.Add(hrNext.InnerText + ", " + hrNext.NextSibling.NextSibling.InnerText);
                                }
                            }
                            string allContactInfo = contactDiv.InnerText;
                            var emailMatch = Regex.Matches(allContactInfo, @"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)");
                            foreach(Match m in emailMatch)
                            {
                                emailList.Add(Utils.CleanChars(m.Value.ToString()));
                            }
                            var phoneMatches = Regex.Matches(allContactInfo, @"Tel. ([0-9\s]+)");
                            foreach (Match m in phoneMatches)
                            {
                                phoneList.Add(Utils.CleanChars(m.Value.ToString()));
                            }
                            var faxMatches = Regex.Matches(allContactInfo, @"Fax. ([0-9\s]+)");
                            foreach (Match m in faxMatches)
                            {
                                faxList.Add(Utils.CleanChars(m.Value.ToString()));
                            }
                            var webMatches = Regex.Matches(allContactInfo, @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})");
                            foreach (Match m in webMatches)
                            {
                                websiteList.Add(Utils.CleanChars(m.Value.ToString()));
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
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
            return null;
        }

    }
}
