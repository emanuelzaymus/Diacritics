using DiacriticsProject1.Reconstructors.TrieDR;

namespace Diacritics
{
    public class InMemoryReconstructor
    {
        TrieDR dr;

        /// <summary>
        /// InMemoryReconstuctor is fast diacritics reconstructor that reconstructs 
        /// one word approximately in 0.2ms with 98,07% accuracy. It needs at least 5GB RAM. 
        /// Creating this object may take several minutes. So create this object only once.
        /// </summary>
        /// <param name="binaryFilePath">Path to binary file "compoundBinFile.dat".</param>
        /// <param name="positionTriePath">Path to text file "positionTrie.txt".</param>
        public InMemoryReconstructor(string binaryFilePath, string positionTriePath)
        {
            dr = new TrieDR(binaryFilePath, positionTriePath);
        }

        /// <summary>
        /// Reconstructs diacritics.
        /// </summary>
        /// <param name="text">Text without diacritics.</param>
        /// <returns>Reconstructed text.</returns>
        public string Reconstruct(string text)
        {
            return dr.Reconstruct(text);
        }

        /// <summary>
        /// Reconstructs text in source file and saves it into destination file.
        /// </summary>
        /// <param name="sourcePath">Path to source text file which contains text without diacritics.</param>
        /// <param name="destinationPath">Path to destination text file where the reconstructed text will be saved. 
        /// If it does not exist then it will be created.</param>
        public void Reconstruct(string sourcePath, string destinationPath)
        {
            dr.Reconstruct(sourcePath, destinationPath);
        }

        /// <summary>
        /// Reconstructs text in source Microsoft Word file and saves it into Microsoft Word destination file.
        /// </summary>
        /// <param name="sourcePath">Path to source Word file (.docx) which contains text without diacritics.</param>
        /// <param name="destinationPath">Path to destination Word file (.docx) where the reconstructed text will be saved. 
        /// If it does not exist then it will be created.</param>
        public void ReconstructWordDocument(string sourcePath, string destinationPath)
        {
            dr.ReconstructWordDocument(sourcePath, destinationPath);
        }

    }
}
