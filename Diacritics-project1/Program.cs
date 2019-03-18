using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors.DBDR;
using DiacriticsProject1.Reconstructors.FileDR;
using DiacriticsProject1.Tester;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DiacriticsProject1.Reconstructors;

[assembly: InternalsVisibleTo("DiacriticsProject1.UnitTests")]

namespace DiacriticsProject1
{
    class Program
    {
        private static List<NgramFile> files = new List<NgramFile>()
        {
            new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy_TO-3_CLEANED_GOOD-WORDS_TO-LENGTH-30.txt"),
            new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy_TO-2_CLEANED_GOOD-WORDS_TO-LENGTH-30.txt"),
            new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy_TO-1_CLEANED_GOOD-WORDS_TO-LENGTH-30.txt"),
            new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS_TO-LENGTH-30.txt")
        };

        private static List<NgramFile> oldFiles = new List<NgramFile>()
        {
            new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
            new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
            new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
            new UniGramFile("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive/prim-8.0-public-img-sk-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt")
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

        private static string[] partialBinFiles =
            {
                "D:/binFiles/prim-8.0-public-all-4-gramy_TO-3_CLEANED_GOOD-WORDS_TO-LENGTH-30_BIN-FILE-FROM-.dat",
                "D:/binFiles/prim-8.0-public-all-3-gramy_TO-2_CLEANED_GOOD-WORDS_TO-LENGTH-30_BIN-FILE-FROM-.dat",
                "D:/binFiles/prim-8.0-public-all-2-gramy_TO-1_CLEANED_GOOD-WORDS_TO-LENGTH-30_BIN-FILE-FROM-.dat",
                "D:/binFiles/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS_TO-LENGTH-30_BIN-FILE-FROM-.dat"
            };

        private static string binaryFilePath = "C:/Users/emanuel.zaymus/Documents/compoundBinFile/compoundBinFile.dat";
        private static string positionTriePath = "C:/Users/emanuel.zaymus/Documents/compoundBinFile/positionTrie.txt";

        static void Main(string[] args)
        {
            //CleanFiles();

            //IDiacriticsReconstructor dr = new TrieDR(oldFiles);
            //DiacriticsTester.Test(testTexts[2], dr);

            //CreateDB();

            //using (var dbdr = new DBDR()) { DiacriticsTester.Test(testTexts[2 - 1], dbdr); }

            //CreatePartialBinaryFiles();
            //CreateCompoundBinaryFile();
            //FileStatistics.BinaryFileNgramStats(binaryFilePath, positionTriePath);

            //FileStatistics.BinaryFilePartitioningStats(binaryFilePath, positionTriePath, "a");

            using (FileDR fdr = new FileDR(binaryFilePath, positionTriePath))
            {
                TestOnTestTexts(fdr);
            }

        }

        private static void CleanFiles()
        {
            var fc = new FileCleaner();

            var file = new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive.txt");
            Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));

            var file2 = new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file2, rmvWordsFromFreq: 1, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));

            var file3 = new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file3, rmvWordsFromFreq: 2, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));
            //var file3 = new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy_TO-2_CLEANED.txt");
            //Console.WriteLine(fc.CompleteProcessing(file3, clean: false, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));

            var file4 = new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy.txt");
            Console.WriteLine(fc.CompleteProcessing(file4, rmvWordsFromFreq: 3, rmvBadWordsFromFreq: 11, rmvWordsFromLength: 30));
        }

        private static void CreateDB()
        {
            DBCreator creator = new DBCreator();
            creator.LoadFiles(files);
        }

        private static void CreatePartialBinaryFiles()
        {
            var fileCreator = new FileCreator();
            fileCreator.CreatePartialBinaryFilesFromFiles(files, "D:/binFiles/");
        }

        private static void CreateCompoundBinaryFile()
        {
            var fileCreator = new FileCreator();
            fileCreator.CreateCompoundBinaryFile(partialBinFiles, binaryFilePath, positionTriePath);
        }

        private static void CreateBinaryFilesFromDB()
        {
            FileCreator.CreateBinaryFileFromDBWordsAndUniGramsEntities("D:/binFiles/positionTrie.txt", "D:/binFiles/fileUniGrams.dat");
        }

        private static void TestOnTestTexts(IDiacriticsReconstructor dr)
        {
            DiacriticsTester.Test(testTexts[1], dr);
            //foreach (var text in testTexts)
            //{
            //    DiacriticsTester.Test(text, dr);
            //}
        }

    }
}
