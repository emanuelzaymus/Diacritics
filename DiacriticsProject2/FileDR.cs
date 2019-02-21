using DiacriticsShared;

namespace DiacriticsProject2
{
    class FileDR : IDiacriticsReconstructor
    {
        private string rootFolder;
        public FileDR(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public string Reconstruct(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
