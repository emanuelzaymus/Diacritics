using System.IO;


namespace DiacriticsProject1
{
    internal class NgramFile : TextFile
    {
        private readonly string path;

        protected StreamReader reader;

        public NgramFile(string path)
        {
            this.path = path;
        }

        internal new string FileName => FileName(path);

        internal new string FileExtension => FileExtension(path);

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
            reader = File.OpenText(path);
        }

    }
}
