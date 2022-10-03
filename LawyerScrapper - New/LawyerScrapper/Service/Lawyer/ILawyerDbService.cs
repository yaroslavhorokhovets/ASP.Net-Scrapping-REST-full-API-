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
        Task<string> InsertLawyerData(List<LawyerDto> lawyerDtos, string lg);
        Task<List<string>> InsertCourtCasesData(List<CourtDetailDto> courtWithNames, string lg);
    }
}
