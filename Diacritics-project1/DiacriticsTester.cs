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
        internal static void Test(string path, DiacriticsReconstructor dr)
        {
            Console.WriteLine($"Reading {path}");
            string originalText = File.OpenText(path).ReadToEnd();

            Console.WriteLine("Removing diacritics...");
            //string textWithoutDiacritics = FileCleaner.RemoveDiacritics(originalText);
            string textWithoutDiacritics = FileCleaner.MyDiacriticsRemover(originalText);

            File.WriteAllText($"{TextFile.FileName(path)}_WITHOUT-DIACRITICS{TextFile.FileExtension(path)}", textWithoutDiacritics);

            Console.WriteLine("Reconstructing...");
            string reconstructedText = dr.Reconstruct(textWithoutDiacritics);
            Console.WriteLine("Done.");

            File.WriteAllText($"{TextFile.FileName(path)}_RENCOSTRUCTED{TextFile.FileExtension(path)}", reconstructedText);

            Console.WriteLine("Testing...");
            findMistakes(originalText, reconstructedText, path);
            Console.WriteLine("Done.\n");
        }

        private static void findMistakes(string originalText, string reconstructedText, string path)
        {
            string[] originalWords = originalText.Split(' ');
            string[] reconstructedWords = reconstructedText.Split(' ');

            Console.WriteLine($"originalWords.Length = {originalWords.Length}");
            Console.WriteLine($"reconstructedWords.Length = {reconstructedWords.Length}");

            int count = 0;

            using (var sw = new StreamWriter($"{TextFile.FileName(path)}_MISTAKES-ORIG-RECONST{TextFile.FileExtension(path)}"))
            {
                for (int i = 0; i < originalWords.Length; i++)
                {
                    if (i < reconstructedWords.Length && originalWords[i] != reconstructedWords[i]) // TODO: out of bound exception
                    {
                        sw.WriteLine($"{originalWords[i]} - {reconstructedWords[i]}");
                        count++;
                    }
                } // TODO: print whats the difference between {originalWords.Length} and {reconstructedWords.Length}
            }

            Console.WriteLine($"Number of mistakes: {count}");
        }

    }
}
