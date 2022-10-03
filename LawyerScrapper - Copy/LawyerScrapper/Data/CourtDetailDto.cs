using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Data
{
    public class CourtDetailDto
    {
        public string CourtCaseNo { get; set; }
        public string HearingGeneral { get; set; }
        public string HearingDate { get; set; }
        public string HearingHour { get; set; }
        public string HearingType { get; set; }
        public string ChamberID { get; set; }
        public string LaywersLink { get; set; }

        public List<string> Lawyers { get; set; }
        public string Country { get; set; }
    }
}
