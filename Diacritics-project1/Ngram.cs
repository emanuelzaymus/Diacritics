using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diacritisc_project1
{
    internal class Ngram
    {
        private readonly string line;
        private readonly NgramFile.Type type;

        public Ngram(string line, NgramFile.Type t)
        {
            this.line = line;
            this.type = t;
        }

        internal string Line
        {
            get => line;
        }

        internal string[] Words
        {
            get
            {
                string[] words;
                string str = line.Trim();
                if (type == NgramFile.Type.Dictionary)
                {
                    words = new string[] { str.Substring(0, str.IndexOf("\t")) };
                }
                else
                {
                    str = str.Substring(str.IndexOf(' ') + 1);
                    words = str.Split('\t');
                }
                return words;
            }
        }

        internal int Frequency
        {
            get
            {
                string frequencyStr = line.Trim();
                if (type == NgramFile.Type.Dictionary)
                {
                    frequencyStr = frequencyStr.Substring(frequencyStr.IndexOf("\t") + 1);
                }
                else
                {
                    frequencyStr = frequencyStr.Substring(0, frequencyStr.IndexOf(' '));
                }
                return Convert.ToInt32(frequencyStr);
            }
        }

    }
}
