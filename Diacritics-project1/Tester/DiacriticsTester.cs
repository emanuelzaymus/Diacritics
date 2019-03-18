using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors;
using System;
using System.Diagnostics;
using System.IO;

namespace DiacriticsProject1.Tester
{
    class DiacriticsTester
    {
        private static string statisticsPath;
        internal static void Test(string path, IDiacriticsReconstructor dr, bool writeStatistics = true)
        {
            long bytes = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory (bytes): {bytes}");
            if (writeStatistics)
            {
                statisticsPath = $"{TextFile.FileName(path)}_STATISTICS{TextFile.FileExtension(path)}";
                File.WriteAllText(statisticsPath, $"Memory (bytes): {bytes}\n");
            }

            Console.WriteLine($"Reading {path}");
            string originalText = File.OpenText(path).ReadToEnd();

            Console.WriteLine("Removing diacritics...");
            string textWithoutDiacritics = StringRoutines.MyDiacriticsRemover(originalText);

            File.WriteAllText($"{TextFile.FileName(path)}_WITHOUT-DIACRITICS{TextFile.FileExtension(path)}", textWithoutDiacritics);

            Console.WriteLine("Reconstructing...");
            var sw = Stopwatch.StartNew();
            string reconstructedText = dr.Reconstruct(textWithoutDiacritics);
            sw.Stop();
            string ngramsStat = dr.GetStatistic();
            Console.Write(ngramsStat);
            if (writeStatistics) { File.AppendAllText(statisticsPath, ngramsStat); }
            dr.EraseStatistic();
            Console.WriteLine($"Elapsed (milliseconds): {sw.Elapsed.TotalMilliseconds}");
            if (writeStatistics) { File.AppendAllText(statisticsPath, $"Elapsed (milliseconds): {sw.Elapsed.TotalMilliseconds}\n"); }
            Console.WriteLine("Done.");

            File.WriteAllText($"{TextFile.FileName(path)}_RENCOSTRUCTED{TextFile.FileExtension(path)}", reconstructedText);

            Console.WriteLine("Testing...");
            FindMistakes(originalText, reconstructedText, path, writeStatistics);
            Console.WriteLine("Done.\n");
        }

        private static void FindMistakes(string originalText, string reconstructedText, string path, bool writeStatistics)
        {
            string[] originalWords = originalText.Split(' ');
            string[] reconstructedWords = reconstructedText.Split(' ');
            // TODO: zapametanie pozicie slova (class Word)

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
            if (writeStatistics)
            {
                File.AppendAllText(statisticsPath, $"originalWords.Length = {originalWords.Length}\n");
                File.AppendAllText(statisticsPath, $"reconstructedWords.Length = {reconstructedWords.Length}\n");
                File.AppendAllText(statisticsPath, $"Number of mistakes: {count}\n");
            }
        }

    }
}
