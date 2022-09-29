using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Helper
{
    public class Utils
    {
        public static string RemoveDutchSpecialChars(string origin)
        {
            return origin.Replace("é", "e").Replace("ü", "u");
        }
        public static string CleanChars(string origin)
        {
            return origin.Replace("\r", "").Replace("\n", "").Trim();
        }
    }
}
