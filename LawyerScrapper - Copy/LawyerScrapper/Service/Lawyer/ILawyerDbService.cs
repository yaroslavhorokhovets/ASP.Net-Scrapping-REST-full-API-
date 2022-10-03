using LawyerScrapper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public interface ILawyerDbService
    {
        Task<string> InsertCourtData(List<CourtDto> courtDtos, string lg);
        Task<int> InsertCourtDetailData(List<CourtDetailDto> courtDtos, string lg);
        List<string> GetDistintFullNames(string lg);
        Task<string> InsertLawyerData(List<LawyerDto> lawyerDtos, string lg);
    }
}
