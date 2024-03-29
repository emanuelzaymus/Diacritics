﻿using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DiacriticsProject1.Tester
{
    class DiacriticsTester
    {
        private static string statisticsPath;

        private static int countOfAllOrigWords = 0;
        private static int countOfAllReconstWords = 0;
        private static int countOfAllMistakes = 0;

        internal static void Test(string path, DiacriticsReconstructor dr, bool writeStatistics = true)
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

        public static void FindMistakes(string originalText, string reconstructedText, string path, bool writeStatistics)
        {
            string[] originalWords = originalText.Split(' ', '\n', '\t', '\r').Where(x => x != "").ToArray();
            string[] reconstructedWords = reconstructedText.Split(' ', '\n', '\t', '\r').Where(x => x != "").ToArray();

            Console.WriteLine($"originalWords.Length = {originalWords.Length}");
            Console.WriteLine($"reconstructedWords.Length = {reconstructedWords.Length}");

            if (originalWords.Length != reconstructedWords.Length)
            {
                //throw new Exception("Length of original and reconstructed text are not equal!");
                Console.WriteLine("Length of original and reconstructed text are not equal!");
            }

            int count = 0;

            using (var sw = new StreamWriter($"{TextFile.FileName(path)}_MISTAKES-RECONST-ORIG{TextFile.FileExtension(path)}"))
            {
                //int len = originalWords.Length;
                int len = Math.Min(originalWords.Length, reconstructedWords.Length);
                for (int i = 0; i < len; i++)
                {
                    var originalW = originalWords[i];
                    var reconstructW = reconstructedWords[i];
                    if (originalW != reconstructW)
                    {
                        sw.WriteLine("{0} {1} {2} {3} {4} {5} {6} - {7} {8} {9} {10} {11} {12} {13}",
                            i - 3 >= 0 ? reconstructedWords[i - 3] : "",
                            i - 2 >= 0 ? reconstructedWords[i - 2] : "",
                            i - 1 >= 0 ? reconstructedWords[i - 1] : "",
                            reconstructW,
                            i + 1 < len ? reconstructedWords[i + 1] : "",
                            i + 2 < len ? reconstructedWords[i + 2] : "",
                            i + 3 < len ? reconstructedWords[i + 3] : "",

                            i - 3 >= 0 ? originalWords[i - 3] : "",
                            i - 2 >= 0 ? originalWords[i - 2] : "",
                            i - 1 >= 0 ? originalWords[i - 1] : "",
                            originalW,
                            i + 1 < len ? originalWords[i + 1] : "",
                            i + 2 < len ? originalWords[i + 2] : "",
                            i + 3 < len ? originalWords[i + 3] : "");

                        count++;
                    }
                }
            }

            Console.WriteLine($"Number of mistakes: {count}");
            if (writeStatistics)
            {
                statisticsPath = $"{TextFile.FileName(path)}_STATISTICS{TextFile.FileExtension(path)}";

                File.AppendAllText(statisticsPath, $"originalWords.Length = {originalWords.Length}\n");
                File.AppendAllText(statisticsPath, $"reconstructedWords.Length = {reconstructedWords.Length}\n");
                File.AppendAllText(statisticsPath, $"Number of mistakes: {count}\n");
            }

            countOfAllOrigWords += originalWords.Length;
            countOfAllReconstWords += reconstructedWords.Length;
            countOfAllMistakes += count;
        }

        public static void PrintOverallStats()
        {
            Console.WriteLine($"countOfAllOrigWords = {countOfAllOrigWords}");
            Console.WriteLine($"countOfAllReconstWords = {countOfAllReconstWords}");
            Console.WriteLine($"countOfAllMistakes = {countOfAllMistakes}");

            Console.WriteLine("Success rate: " + ( 100 - ((double)countOfAllMistakes / countOfAllOrigWords) * 100));
        }

    }
}
