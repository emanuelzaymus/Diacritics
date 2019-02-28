using System;

namespace DiacriticsProject1.Common.Ngrams
{
    internal class UniGram : Ngram
    {
        public UniGram(string line) : base(line)
        {
        }

        internal override string[] Words
        {
            get
            {
                string str = Line.Trim();
                return new string[] { str.Substring(0, str.IndexOf("\t")) };
            }
        }

        internal override int Frequency
        {
            get
            {
                string frequencyStr = Line.Trim();
                frequencyStr = frequencyStr.Substring(frequencyStr.IndexOf("\t") + 1);
                return Convert.ToInt32(frequencyStr);
            }
        }

        public override string ToString()
        {
            string str = Line.Trim();
            return str.Substring(0, str.IndexOf("\t"));
        }
    }
}
