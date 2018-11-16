using Diacritics_project1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diacritisc_project1
{
    class NgramsFileCleaner : FileCleaner
    {
        internal void Clean(string path)
        {
            string line;
            int i = 500000;

            using (StreamReader sr = File.OpenText(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string[] words = getWords(line);

                    foreach (var w in words)
                    {
                        if (!Regex.IsMatch(w, $@"^{charsPattern}+$") && !Regex.IsMatch(w, digitsPattern))
                        {
                            Console.WriteLine($"{line}");
                        }
                    }

                    if (--i <= 0) return;
                }
            }
        }

        private string[] getWords(string line)
        {
            line = line.Trim();
            line = line.Substring(line.IndexOf(' ') + 1);
            return line.Split('\t');
        }
    }
}
