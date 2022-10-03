using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Data
{
    public class AvocatsDto
    {
        public string Name { get; set; }
        public string LawyerInfoSection { get; set; }
        public double AddressLatitude { get; set; }
        public double AddressLongitude { get; set; }
        
    }
    public class AvocatsResults
    {
        public int Pages { get; set; }
        public List<AvocatsDto> Results { get; set; }
    }
}
