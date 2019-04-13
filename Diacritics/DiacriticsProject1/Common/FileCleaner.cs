using System.Text.RegularExpressions;

namespace DiacriticsProject1.Common
{
    internal class FileCleaner
    {
        private static string latinChars = "a-z";
        private static string nonLatinChars = "áäčďéíĺľňóôŕšťúýžěřůöüẞß";

        public static Regex rgxChars = new Regex($"[{latinChars}{nonLatinChars}]");
        public static Regex rgxNonLatinChars = new Regex($"[{nonLatinChars}]");
        
    }
}