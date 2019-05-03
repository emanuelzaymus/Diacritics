using System;
using System.IO;
using System.Linq;

namespace Diacritics.Tester
{
    public class DiacriticsTester
    {

        public static void FindMistakes(string originalTextPath, string reconstructedTextPath, bool writeStatictics)
        {
            string originalText = File.ReadAllText(originalTextPath);
            string reconstructedText = File.ReadAllText(reconstructedTextPath);

            FindMistakes(originalText, reconstructedText, reconstructedTextPath, writeStatictics);
        }

        public static void FindMistakes(string originalText, string reconstructedText, string path, bool writeStatistics)
        {
            string[] originalWords = originalText.Split(' ', '\n', '\t', '\r').Where(x => x != "").ToArray();
            string[] reconstructedWords = reconstructedText.Split(' ', '\n', '\t', '\r').Where(x => x != "").ToArray();

            if (originalWords.Length != reconstructedWords.Length)
            {
                Console.WriteLine("Length of original and reconstructed text are not equal!");
                Console.WriteLine($"Count of words in original text = {originalWords.Length}");
                Console.WriteLine($"Count of words in reconstructed text = {reconstructedWords.Length}");
            }

            int count = 0;

            using (var sw = new StreamWriter($"{TextFile.FileName(path)}_MISTAKES-ORIG-RECONST{TextFile.FileExtension(path)}"))
            {
                int len = Math.Min(originalWords.Length, reconstructedWords.Length);
                for (int i = 0; i < len; i++)
                {
                    var originalW = originalWords[i];
                    var reconstructW = reconstructedWords[i];
                    if (originalW != reconstructW)
                    {
                        sw.WriteLine("{0} {1} {2} {3} {4} {5} {6} - {7} {8} {9} {10} {11} {12} {13}",
                            i - 3 >= 0 ? originalWords[i - 3] : "",
                            i - 2 >= 0 ? originalWords[i - 2] : "",
                            i - 1 >= 0 ? originalWords[i - 1] : "",
                            originalW,
                            i + 1 < len ? originalWords[i + 1] : "",
                            i + 2 < len ? originalWords[i + 2] : "",
                            i + 3 < len ? originalWords[i + 3] : "",

                            i - 3 >= 0 ? reconstructedWords[i - 3] : "",
                            i - 2 >= 0 ? reconstructedWords[i - 2] : "",
                            i - 1 >= 0 ? reconstructedWords[i - 1] : "",
                            reconstructW,
                            i + 1 < len ? reconstructedWords[i + 1] : "",
                            i + 2 < len ? reconstructedWords[i + 2] : "",
                            i + 3 < len ? reconstructedWords[i + 3] : "");
                        count++;
                    }
                }
            }

            if (writeStatistics)
            {
                var statisticsPath = $"{TextFile.FileName(path)}_STATISTICS{TextFile.FileExtension(path)}";

                File.AppendAllText(statisticsPath, $"Count of words in original text = {originalWords.Length}\n");
                File.AppendAllText(statisticsPath, $"Count of words in reconstructed text = {reconstructedWords.Length}\n");
                File.AppendAllText(statisticsPath, $"Number of mistakes: {count}\n");
            }
        }

    }
}
