using LawyerScrapper.Data;
using LawyerScrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public interface IAdvocaatScraper : IDisposable
    {
        Task<LawyerDto> GetLawyerDetail(string fullName);
    }
}
