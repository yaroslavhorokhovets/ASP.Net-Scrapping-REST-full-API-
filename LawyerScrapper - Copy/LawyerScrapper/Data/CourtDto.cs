using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Data
{
    public class CourtDto
    {
        public string Canton { get; set; }
        public string Codification { get; set; }
        public string Division { get; set; }

        [JsonProperty("division_id")]
        public string DivisionId { get; set; }
        public string Id { get; set; }
        [JsonProperty("jur_annexe")]
        public string JurAnnexe { get; set; }
        [JsonProperty("jur_num")]
        public string JurNum { get; set; }
        [JsonProperty("type_juridiction")]
        public string TypeJuridiction { get; set; }
        [JsonProperty("type_juridiction_id")]
        public int TypeJuridictionId { get; set; }

        public List<string> AvailableDates { get; set; }
        public string Country { get; set; }

    }

    public class CourtResultDto
    {
        public List<CourtDto> Data { get; set; }
        public string Message { get; set; }
    }
}
