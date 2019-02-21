namespace DiacriticsProject1.Reconstructors.FileDR
{
    class NgramFileDR : DRBase
    {
        private string rootFolder;
        public NgramFileDR(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            throw new System.NotImplementedException();
        }
    }
}
