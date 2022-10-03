using HtmlAgilityPack;
using LawyerScrapper.Data;
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
    public class DossierScraper : IDossierScraper
    {
        private RestClient _client;
        public DossierScraper() { 
            _client = new RestClient("https://dossier.just.fgov.be");
        }

        public void Dispose()
        {
            
        }
        public async Task<List<CourtDto>> GetCourtList(string searchString, string lg)
        {
            var request = new RestRequest("cgi-main/ajax-request-json.pl");
            request.AddParameter("requete", "json_list");
            request.AddParameter("lg", lg);
            request.AddParameter("liste", "juridiction");
            request.AddParameter("backend", "N");
            request.AddParameter("search", searchString);
            
            var response = await _client.GetAsync(request);
            var results = JsonConvert.DeserializeObject<CourtResultDto>(response.Content);
            if (results != null && results.Data.Count  > 0)
            {
                return results.Data;
            }
            return null;
        }

        public async Task<List<string>> GetAvailableDates(CourtDto courtDto, string lg)
        {
            var request = new RestRequest("cgi-dossier/ajax-request-html.pl");
            request.AddParameter("request", "show_fixations_form");
            request.AddParameter("key", $"{courtDto.JurNum}--{courtDto.JurAnnexe}--{lg}");
           
            var response = await _client.GetAsync(request);
            var matches = Regex.Matches(response.Content, @"var dates = (.*?);");
            string datesString = "";
            foreach (Match m in matches)
            {
                datesString = m.Value.ToString().Replace("var dates = ", "").Replace(";", "");
            }
            if (!string.IsNullOrEmpty(datesString))
            {
                return JsonConvert.DeserializeObject<List<string>>(datesString);
            }
            return null;
        }

        // Get CourtDetail list from one Court (speicfic date + specifict court)
        public async Task<List<CourtDetailDto>> GetSpecificDateCourtCaseList(CourtDto court, string availableDate, string lg)
        {
            DateTime specificDate = DateTime.ParseExact(availableDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var request = new RestRequest("cgi-dossier/ajax-request-html.pl");
            request.AddParameter("request", "show_fixations_list");
            request.AddParameter("lg", lg);
            request.AddParameter("jur_num", court.JurNum);
            request.AddParameter("jur_annexe", court.JurAnnexe);
            request.AddParameter("date_audience", specificDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            
            var response = await _client.GetAsync(request);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);
            
            var hearingInfo = doc.DocumentNode.SelectSingleNode("//h4").InnerText;
            var tbody = doc.DocumentNode.SelectSingleNode("//tbody");

            var courts = new List<CourtDetailDto>();
            var trs = tbody.SelectNodes(".//tr");
            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes(".//td");
                    if (tds != null)
                    {
                        string courtCaseNo;
                        string hearingDate;
                        string hearingHour;
                        string hearingType;
                        string chamberId;
                        string laywersLink;

                        if (tds.Count >= 8)
                        {
                            courtCaseNo = tds[0].InnerText + ", " + tds[1].InnerText + ", " + tds[2].InnerText;
                            hearingDate = tds[3].InnerText;
                            hearingHour = tds[4].InnerText;
                            chamberId = tds[5].InnerText;
                            hearingType = tds[6].InnerText;
                            laywersLink = tds[7].SelectSingleNode(".//a").Attributes["href"].Value;
                        }
                        else
                        {
                            courtCaseNo = tds[0].InnerText;
                            hearingDate = tds[1].InnerText;
                            hearingHour = tds[2].InnerText;
                            chamberId = tds[3].InnerText;
                            hearingType = tds[4].InnerText;
                            if (tds.Count == 6)
                            {
                                laywersLink = tds[5].SelectSingleNode(".//a").Attributes["href"].Value;
                            }
                            else
                            {
                                laywersLink = "";
                            }

                        }

                        courts.Add(new CourtDetailDto
                        {
                            HearingGeneral = hearingInfo,
                            CourtCaseNo = courtCaseNo,
                            HearingDate = hearingDate,
                            HearingHour = hearingHour,
                            ChamberID = chamberId,
                            HearingType = hearingType,
                            LaywersLink = laywersLink,
                            Country = lg,
                        });
                    }
                    
                }
            }
            
            return courts;
        }
        // Get Lawyer Names from CourtDetail
        public async Task<CourtDetailDto> GetLawyerNamesFromCourt(CourtDetailDto dto)
        {
            dto.Lawyers = new List<string>();
            var request = new RestRequest(dto.LaywersLink);
            var response = await _client.GetAsync(request);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);
            var divLawyerNames = doc.DocumentNode.SelectNodes("//div[contains(@class, 'col-lg-3 col-md-3 col-sm-3')]");
            if (divLawyerNames != null)
            {
                foreach (var divLawyerName in divLawyerNames)
                {
                    dto.Lawyers.Add(divLawyerName.InnerText);
                }
            }
            return dto;
        }
    }
}
