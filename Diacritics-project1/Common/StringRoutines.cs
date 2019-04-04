using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DiacriticsProject1.Common
{
    class StringRoutines
    {
        private static readonly Dictionary<char, char> letters = new Dictionary<char, char>()
        {
            { 'á', 'a' }, { 'ä', 'a' }, { 'č', 'c' }, { 'ď', 'd' }, { 'é', 'e' }, { 'í', 'i' },
            { 'ĺ', 'l' }, { 'ľ', 'l' }, { 'ň', 'n' }, { 'ó', 'o' }, { 'ô', 'o' }, { 'ŕ', 'r' },
            { 'š', 's' }, { 'ť', 't' }, { 'ú', 'u' }, { 'ý', 'y' }, { 'ž', 'z' }, { 'ě', 'e' },
            { 'ř', 'r' }, { 'ů', 'u' }, { 'ö', 'o' }, { 'ü', 'u' },
            { 'Á', 'A' }, { 'Ä', 'A' }, { 'Č', 'C' }, { 'Ď', 'D' }, { 'É', 'E' }, { 'Í', 'I' },
            { 'Ĺ', 'L' }, { 'Ľ', 'L' }, { 'Ň', 'N' }, { 'Ó', 'O' }, { 'Ô', 'O' }, { 'Ŕ', 'R' },
            { 'Š', 'S' }, { 'Ť', 'T' }, { 'Ú', 'U' }, { 'Ý', 'Y' }, { 'Ž', 'Z' }, { 'Ě', 'E' },
            { 'Ř', 'R' }, { 'Ů', 'U' }, { 'Ö', 'O' }, { 'Ü', 'U' }
        };

        private static StringBuilder stringBuilder = new StringBuilder();

        public static string RemoveDiacritics(string word)
        {
            var normalizedString = word.Normalize(NormalizationForm.FormD);
            stringBuilder.Clear();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string MyDiacriticsRemover(string word)
        {
            char outCh;
            stringBuilder.Clear();
            foreach (var ch in word)
            {
                if (letters.TryGetValue(ch, out outCh))
                {
                    stringBuilder.Append(outCh);
                }
                else
                {
                    stringBuilder.Append(ch);
                }
            }
            return stringBuilder.ToString();
        }

        public static string Normalize(string str)
        {
            //return str.ToLower();
            return str.Replace('"', ' ').Replace('„', ' ').Replace('“', ' ').Replace('”', ' ').Replace('\'', ' ').Replace('`', ' ')
                .Replace('‘', ' ').Replace('’', ' ').Replace('…', '.').Replace(':', ' ');
            //.Replace('—', ' ').Replace('–', ' ').Replace('-', ' ');
            //return string.Join(" ", str.Split(' ', '\t', '\n', '\r').Where(x => x != "").ToArray());
        }

    }
}
