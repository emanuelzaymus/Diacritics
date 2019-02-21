using System;

namespace DiacriticsProject1.Common.Ngrams
{
    internal class Ngram
    {

        public Ngram(string line)
        {
            this.Line = line;
        }

        internal string Line { get; }

        internal virtual string[] Words
        {
            get
            {
                string str = Line.Trim();
                str = str.Substring(str.IndexOf(' ') + 1);
                return str.Split('\t');
            }
        }

        internal virtual int Frequency
        {
            get
            {
                string frequencyStr = Line.Trim();
                frequencyStr = frequencyStr.Substring(0, frequencyStr.IndexOf(' '));
                return Convert.ToInt32(frequencyStr);
            }
        }
    }
}
