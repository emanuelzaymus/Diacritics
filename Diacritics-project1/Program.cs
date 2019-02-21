using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors;
using DiacriticsProject1.Reconstructors.FileDR;
using DiacriticsProject1.Tester;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DiacriticsProject1.UnitTests")]

namespace DiacriticsProject1
{
    class Program
    {
        private static string[] testTexts =
            {
                "D:/testovacie_texty/1/Retz.txt",
                "D:/testovacie_texty/2/Ako sa eseťákom pokúsili ukradnúť hotel Carlton.txt",
                "D:/testovacie_texty/3/Macron ako novodobý Ľudovít XVI.txt",
                "D:/testovacie_texty/4/Bobby Robson.txt",
                "D:/testovacie_texty/5/Mor ho.txt",
                "D:/testovacie_texty/6/PETER HOTRA.txt"
            };

        private static string wordsFilesRootFolder = "D:/words";

        static void Main(string[] args)
        {
            CleanFiles();

            IDiacriticsReconstructor dr = new TrieDR();

            DiacriticsTester.Test(testTexts[2 - 1], dr);
        }

        private static void CleanFiles()
        {
            var fc = new FileCleaner();

            var file = new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive.txt");
            Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11));

            var file2 = new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file2, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));

            var file3 = new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file3, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));

            var file4 = new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file4, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));
        }

    }
}
