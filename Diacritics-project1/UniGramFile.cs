using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Diacritisc_project1
{
    class UniGramFile : NgramFile
    {
        public UniGramFile(string path) : base(path)
        {
        }

        internal override Ngram Next()
        {
            if (reader == null)
            {
                ReOpen();
            }
            string line = reader.ReadLine();
            return (line != null) ? new UniGram(line) : null;
        }
    }
}
