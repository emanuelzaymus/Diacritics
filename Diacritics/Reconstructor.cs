using DiacriticsProject1.Reconstructors.FileDR;
using System;

namespace Diacritics
{
    public class Reconstructor : IDisposable
    {
        private FileDR dr;

        /// <summary>
        /// Reconstuctor is fast diacritics reconstructor that reconstructs 
        /// one word approximately in 4ms with 98,17% accuracy. It needs at least 1.5GB RAM. 
        /// Creating this object may take few seconds. It is recommended to create this object only once.
        /// </summary>
        /// <param name="binaryFilePath">Path to binary file "compoundBinFile.dat".</param>
        /// <param name="positionTriePath">Path to text file "positionTrie.txt".</param>
        public Reconstructor(string binaryFilePath, string positionTriePath)
        {
            dr = new FileDR(binaryFilePath, positionTriePath);
        }

        /// <summary>
        /// Reconstructs diacritics.
        /// </summary>
        /// <param name="text">Text without diacritics.</param>
        /// <returns>Reconstructed text. If input string text is null then returns null.</returns>
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

        /// <summary>
        /// Disposes the Reconstructor.
        /// </summary>
        public void Dispose()
        {
            dr.Dispose();
        }

    }
}
