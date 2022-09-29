using LawyerScrapper.Data;
using LawyerScrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public interface IDossierScraper : IDisposable
    {
        Task<List<CourtDto>> GetCourtList(string searchString, string lg);

        Task<List<string>> GetAvailableDates(CourtDto courtDto, string lg);

        Task<List<CourtDetailDto>> GetSpecificDateCourtList(CourtDto court, string availableDate, string lg);

        Task<CourtDetailDto> GetLawyerNamesFromCourt(CourtDetailDto courtDto);
    }
}
