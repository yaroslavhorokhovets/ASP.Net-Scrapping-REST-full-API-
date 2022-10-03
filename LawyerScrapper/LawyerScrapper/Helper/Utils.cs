using LawyerScrapper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// SECTIONS CIVIL ET FAMILLE SECTIES BURGERLIJKE EN FAMILIE
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

    public class Constants
    {
        public static readonly string today = "2022-10-03";
        // public static readonly string today = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        public static readonly string[] headerList = {
            // Dutch
            "VREDEGERECHT THUIN",
            "POLITIERECHTBANK ANTWERPEN AFDELING ANTWERPEN",
            "RECHTBANK VAN EERSTE AANLEG ANTWERPEN AFDELING ANTWERPEN, SECTIES BURGERLIJKE EN FAMILIE",
            "ARBEIDSRECHTBANK ANTWERPEN AFDELING ANTWERPEN",
            "ONDERNEMINGSSRECHTBANK ANTWERPEN AFDELING ANTWERPEN",
            "HOF VAN BEROEP ANTWERPEN AFDELING ANTWERPEN",
            "ARBEIDSHOF ANTWERPEN AFDELING ANTWERPEN",
            "HOF VAN ASSISEN ANTWERPEN AFDELING ANTWERPEN",
            "HOF VAN CASSATIE ANTWERPEN AFDELING ANTWERPEN",
            // French
            "JUSTICE DE PAIX THUIN",
            "TRIBUNAL DE POLICE ANVERS DIVISION ANVERS",
            "TRIBUNAL DE PREMIERE INSTANCE ANVERS DIVISION ANVERS, SECTIONS CIVIL ET FAMILLE",
            "TRIBUNAL DU TRAVAIL ANVERS DIVISION ANVERS",
            "TRIBUNAL DE L'ENTREPRISE ANVERS DIVISION ANVERS",
            "COUR D'APPEL ANVERS DIVISION ANVERS",
            "COUR DU TRAVAIL ANVERS DIVISION ANVERS",
            "COUR D'ASSISES ANVERS DIVISION ANVERS",
            "COUR DE CASSATION ANVERS DIVISION ANVERS"
        };
        public static string generateCourtType(string origin)
        {
            // Dutch
            if (origin.Trim().StartsWith("VREDEGERECHT"))
            {
                return "VREDEGERECHT";
            }
            else if (origin.Trim().StartsWith("POLITIERECHTBANK")) {
                return "POLITIERECHTBANK";
            }
            else if (origin.Trim().StartsWith("RECHTBANK VAN EERSTE AANLEG")) {
                return "RECHTBANK VAN EERSTE AANLEG";
            }
            else if (origin.Trim().StartsWith("ARBEIDSRECHTBANK")) {
                return "ARBEIDSRECHTBANK";
            }
            else if (origin.Trim().StartsWith("ONDERNEMINGSSRECHTBANK")) {
                return "ONDERNEMINGSSRECHTBANK";
            }
            else if (origin.Trim().StartsWith("HOF VAN BEROEP")) {
                return "HOF VAN BEROEP";
            }
            else if (origin.Trim().StartsWith("ARBEIDSHOF")) {
                return "ARBEIDSHOF";
            }
            else if (origin.Trim().StartsWith("HOF VAN ASSISEN")) {
                return "HOF VAN ASSISEN";
            }
            else if (origin.Trim().StartsWith("HOF VAN CASSATIE")) {
                return "HOF VAN CASSATIE";
            }
            // French
            else if (origin.Trim().StartsWith("JUSTICE DE PAIX")) {
                return "JUSTICE DE PAIX";
            }
            else if (origin.Trim().StartsWith("TRIBUNAL DE POLICE")) {
                return "TRIBUNAL DE POLICE";
            }
            else if (origin.Trim().StartsWith("TRIBUNAL DE PREMIERE INSTANCE")) {
                return "TRIBUNAL DE PREMIERE INSTANCE";
            }
            else if (origin.Trim().StartsWith("TRIBUNAL DU TRAVAIL")) {
                return "TRIBUNAL DU TRAVAIL";
            }
            else if (origin.Trim().StartsWith("TRIBUNAL DE L'ENTREPRISE")) {
                return "TRIBUNAL DE L'ENTREPRISE";
            }
            else if (origin.Trim().StartsWith("COUR D'APPEL")) {
                return "COUR D'APPEL";
            }
            else if (origin.Trim().StartsWith("COUR DU TRAVAIL")) {
                return "COUR DU TRAVAIL";
            }
            else if (origin.Trim().StartsWith("COUR D'ASSISES")) {
                return "COUR D'ASSISES";
            }
            else if (origin.Trim().StartsWith("COUR DE CASSATION")) {
                return "COUR DE CASSATION";
            }
            else
            {
                return origin.Substring(0, origin.IndexOf(", "));
            }
        }
    }

}
