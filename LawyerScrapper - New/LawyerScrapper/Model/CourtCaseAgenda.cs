using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Model
{
    public class CourtCaseAgenda
    {
        public int ID { get; set; }
        public string CourtCaseNo { get; set; }
        public string HearingGeneral { get; set; }
        public string HearingDateTime { get; set; }
        public string HearingType { get; set; }
        public string CourtType { get; set; }
        public string ChamberID { get; set; }
        public string CourtLocation { get; set; }
        public string LawyerName { get; set; }
        public string Country { get; set; }

    }
}
