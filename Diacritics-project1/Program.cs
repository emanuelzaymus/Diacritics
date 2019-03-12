using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors.DBDR;
using DiacriticsProject1.Reconstructors.FileDR;
using DiacriticsProject1.Tester;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DiacriticsProject1.UnitTests")]

namespace DiacriticsProject1
{
    class Program
    {
        private static List<NgramFile> files = new List<NgramFile>()
            {
                //new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/skuska3.txt")
                //new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/skuska2.txt")
                //new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/skuska1.txt")
                //new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS_TO-LENGTH-30_milion.txt")


                //new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS_TO-LENGTH-30.txt")
            };


        private static string[] testTexts =
            {
                "D:/testovacie_texty/1/Retz.txt",
                "D:/testovacie_texty/2/Ako sa eseťákom pokúsili ukradnúť hotel Carlton.txt",
                "D:/testovacie_texty/3/Macron ako novodobý Ľudovít XVI.txt",
                "D:/testovacie_texty/4/Bobby Robson.txt",
                "D:/testovacie_texty/5/Mor ho.txt",
                "D:/testovacie_texty/6/PETER HOTRA.txt"
            };

        static void Main(string[] args)
        {
            //CleanFiles();

            //IDiacriticsReconstructor dr = new TrieDR();
            //DiacriticsTester.Test(testTexts[2 - 1], dr);

            //CreateDB();

            //using (var dbdr = new DBDR()) { DiacriticsTester.Test(testTexts[2 - 1], dbdr); }

            //FileCreator.CreateBinaryFile();
            //FileCreator.Test(files[0].Path);

            using (FileDR fdr = new FileDR()) { DiacriticsTester.Test(testTexts[2 - 1], fdr); }

        }

        private static void CleanFiles()
        {
            var fc = new FileCleaner();

            var file = new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive.txt");
            Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));

            var file2 = new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file2, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));

            var file3 = new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file3, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));

            var file4 = new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file4, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11));
        }

        private static void CreateDB()
        {
            DBCreator creator = new DBCreator();
            creator.LoadFiles(files);
        }

    }
}
