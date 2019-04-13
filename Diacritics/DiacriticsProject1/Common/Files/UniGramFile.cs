using DiacriticsProject1.Common.Ngrams;

namespace DiacriticsProject1.Common.Files
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
