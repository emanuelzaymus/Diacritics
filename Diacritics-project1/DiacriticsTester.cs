using Diacritics_project1;
using System;
using System.Collections.Generic;
using System.IO;
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
            string originalText = File.OpenText(path).ReadToEnd();

            Console.WriteLine("Removing diacritics...");
            string textWithoutDiacritics = FileCleaner.RemoveDiacritics(originalText);

            File.WriteAllText($"{TextFile.FileName(path)}_WITHOUT-DIACRITICS{TextFile.FileExtension(path)}", textWithoutDiacritics);

            DiacriticsReconstructor dr = new DiacriticsReconstructor();

            Console.WriteLine("Reconstructing...");
            string reconstructedText = dr.Reconstruct(textWithoutDiacritics);
            Console.WriteLine("Done.");

            File.WriteAllText($"{TextFile.FileName(path)}_RENCOSTRUCTED{TextFile.FileExtension(path)}", reconstructedText);

            Console.WriteLine("Testing...");
            findMistakes(originalText, reconstructedText);
            Console.WriteLine("Done.");
        }

        private static void findMistakes(string originaltext, string reconstructedText) // DOTO: probably bug
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

    }
}
