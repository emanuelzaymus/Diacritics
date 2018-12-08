using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Diacritisc_project1
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
    }
}
