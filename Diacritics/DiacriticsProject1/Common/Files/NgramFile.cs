using DiacriticsProject1.Common.Ngrams;
using System.IO;

namespace DiacriticsProject1.Common.Files
{
    internal class NgramFile : TextFile
    {
        public string Path { get; }

        protected StreamReader reader;

        public NgramFile(string path)
        {
            this.Path = path;
        }

        internal new string FileName => FileName(Path);

        internal new string FileExtension => FileExtension(Path);

        internal virtual Ngram Next()
        {
            if (reader == null)
            {
                ReOpen();
            }
            string line = reader.ReadLine();
            return (line != null) ? new Ngram(line) : null;
        }

        internal void ReOpen()
        {
            reader = File.OpenText(Path);
        }

    }
}
