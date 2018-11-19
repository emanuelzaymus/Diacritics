using Diacritics_project1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diacritisc_project1
{
    class DiacriticsTester
    {
        internal static void Test(string path)
        {
            Console.WriteLine("Reading...");
            string originalText = FileHandler.ReadUTF8(path);

            Console.WriteLine("Removing diacritics...");
            string textWithoutDiacritics = FileCleaner.RemoveDiacritics(originalText);

            FileHandler.WriteUTF8($"{fileName(path)}_WITHOUT-DIACRITICS{fileExtension(path)}", textWithoutDiacritics);

            DiacriticsReconstructor dr = new DiacriticsReconstructor();

            Console.WriteLine("Reconstructing...");
            string reconstructedText = dr.Reconstruct(textWithoutDiacritics);
            Console.WriteLine("Done.");

            FileHandler.WriteUTF8($"{fileName(path)}_RENCOSTRUCTED{fileExtension(path)}", reconstructedText);

            Console.WriteLine("Testing...");
            findMistakes(originalText, reconstructedText);
            Console.WriteLine("Done.");
        }

        private static void findMistakes(string originaltext, string reconstructedText)
        {
            string[] originalWords = originaltext.Split(' ');
            string[] reconstructedWords = reconstructedText.Split(' ');
            int count = 0;

            Console.WriteLine("Mistakes:\nOriginal - Reconstructed");
            for (int i = 0; i < originalWords.Length; i++)
            {
                if (i < reconstructedWords.Length && originalWords[i] != reconstructedWords[i]) // TODO: out of bound exception
                {
                    Console.WriteLine($"{originalWords[i]} - {reconstructedWords[i]}");
                    count++;
                }
            }

            Console.WriteLine($"\nNumber of mistakes: {count}");
        }

        private static string fileName(string path) => path.Substring(0, path.LastIndexOf('.')); // TODO: duplicita !!!

        private static string fileExtension(string path) => path.Substring(path.LastIndexOf('.')); // TODO: duplicita !!!


    }
}
