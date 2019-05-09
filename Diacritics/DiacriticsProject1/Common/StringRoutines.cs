using System.Collections.Generic;
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

        private StringBuilder stringBuilder = new StringBuilder();

        public string MyDiacriticsRemover(string word)
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

    }
}
