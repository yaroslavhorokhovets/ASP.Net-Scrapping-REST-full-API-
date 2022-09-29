using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Model
{
    public class Court
    {
        public int ID { get; set; }
        public string Canton { get; set; }
        public string Codification { get; set; }
        public string Division { get; set; }

        public string DivisionId { get; set; }
        public string CourtId { get; set; }
        public string JurAnnexe { get; set; }
        public string JurNum { get; set; }
        public string TypeJuridiction { get; set; }
        public int TypeJuridictionId { get; set; }
        public string Country { get; set; }

    }
}
