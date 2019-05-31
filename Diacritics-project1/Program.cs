using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Reconstructors.DBDR;
using DiacriticsProject1.Reconstructors.FileDR;
using DiacriticsProject1.Reconstructors.TrieDR;
using DiacriticsProject1.Tester;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DiacriticsProject1.Reconstructors;
using System.IO;

[assembly: InternalsVisibleTo("DiacriticsProject1.UnitTests")]

namespace DiacriticsProject1
{
    class Program
    {
        private static string[] allTexts =
        {
            "/odborne/astronomia/Astronomia.txt",
            "/odborne/bobby robson/Bobby Robson.txt",
            "/odborne/erozia/Erozia.txt",
            "/odborne/etika a genove inzinierstvo/Etika a genove inzinierstvo.txt",
            "/odborne/mimoplucne tbc/Mimoplucne TBC.txt",
            "/odborne/staroveky rim/Staroveky Rim.txt",
            "/odborne/vcela medonosna/Vcela medonosna.txt",

            "/publicisticke/ako sa esetakom pokusili ukradnut hotel carlton/Ako sa esetakom pokusili ukradnut hotel Carlton.txt",
            "/publicisticke/bratislavsky bikesharing predstavil novy cennik/Bratislavsky bikesharing predstavil novy cennik.txt",
            "/publicisticke/domaca skola ju naucila s radostou sluzit druhym/Domaca skola ju naucila s radostou sluzit druhym.txt",
            "/publicisticke/huawei p30 pro oficialne predstaveny/Huawei P30 Pro oficialne predstaveny.txt",
            "/publicisticke/macron ako novodoby ludovit xvi/Macron ako novodoby Ludovit XVI.txt",
            "/publicisticke/najlepsi politicky film sucasnosti/Najlepsi politicky film sucasnosti.txt",
            "/publicisticke/nato ma 70 rokov/NATO ma 70 rokov.txt",
            "/publicisticke/paci sa mi robit humor bez nadavok/Paci sa mi robit humor bez nadavok.txt",
            "/publicisticke/retz/Retz.txt",
            "/publicisticke/rokovania s usa budu pokracovat/Rokovania s USA budu pokracovat.txt",
            "/publicisticke/smrt domorodych americanov po prichode europanov ochladila planetu/Smrt domorodych Americanov po prichode Europanov ochladila planetu.txt",
            "/publicisticke/smutna premierka na odchode/Smutna premierka na odchode.txt",
            "/publicisticke/ukrajinske volby/Ukrajinske volby.txt",
            "/publicisticke/vedecka fantastika je predobrazom toho co je mozne/Vedecka fantastika je predobrazom toho co je mozne.txt",

            "/umelecke/banality/Banality.txt",
            "/umelecke/maco mliec/Maco Mliec.txt",
            "/umelecke/mor ho/Mor ho.txt",
            "/umelecke/tapakovci/Tapakovci.txt",
        };

        private static string[] reconstructorsPath =
        {
            "D:/testovacie_texty/FileDR",
            "D:/testovacie_texty/TrieDR",
            "D:/testovacie_texty/DiakritikKorpus",
            "D:/testovacie_texty/TextFiitStuba",
            "D:/testovacie_texty/DiakritikaBrmSk",
        };

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
                "D:/testovacie_texty/6/PETER HOTRA.txt",
                "D:/testovacie_texty/7/urls.txt"
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

        private static string diacriticsOrig = "D:/testovacie_texty/4/Bobby Robson_pre_diacritics.txt";
        private static string diacritics = "D:/testovacie_texty/4/Diacritics/Diacritics.txt";

        private static string wiki_path_AA = "D:/OutputWikiData/AA/";
        private static string wiki_path_AB = "D:/OutputWikiData/AB/";
        private static string wiki_path_AC = "D:/OutputWikiData/AC/";

        private static int wiki_AA_count = 100;
        private static int wiki_AB_count = 100;
        private static int wiki_AC_count = 38;

        private static string wiki_ = "wiki_";

        static void Main(string[] args)
        {
            //TestOnTestTexts(new TrieDR(binaryFilePath, positionTriePath), 1);

            //TrieDR trieDR = new TrieDR(binaryFilePath, positionTriePath);
            //TestWiki(trieDR);

            using (FileDR dr = new FileDR(binaryFilePath, positionTriePath))
            {
                TestWiki(dr);
            }

            //using (FileDR fdr = new FileDR(binaryFilePath, positionTriePath))
            //{
            //    TestOnTestTexts(fdr, 0);
            //}

            //OnlyTestTexts(reconstructorsPath[4]);
            //OnlyTestTexts(reconstructorsPath[3]);
            //OnlyTestTexts(reconstructorsPath[2]);
        }

        private static void Normalize(string rPath)
        {
            foreach (var text in allTexts)
            {
                string t = File.OpenText(rPath + text).ReadToEnd();
                t = StringRoutines.Normalize(t);
                File.WriteAllText(TextFile.FileName(rPath + text) + "_NORMALIZED.txt", t);
            }
        }

        private static void CreateNonDiacriticsFiles(string rPath)
        {
            foreach (var text in allTexts)
            {
                //string path = rPath + TextFile.FileName(text) + "_NORMALIZED" + TextFile.FileExtension(text);
                string path = rPath + text;
                FileCleaner.RemoveDiacriticsInFile(path);

                //File.Create(TextFile.FileName(rPath + text) + "_Reconstructed.txt");
            }
        }

        private static void OnlyTestTexts(string rPath)
        {
            foreach (var text in allTexts)
            {
                //string originalText = File.OpenText(rPath + TextFile.FileName(text) + "_NORMALIZED.txt").ReadToEnd();
                string originalText = File.OpenText(rPath + text).ReadToEnd();
                //originalText = StringRoutines.Normalize(originalText);
                string reconstructedText = File.OpenText(rPath + TextFile.FileName(text) + "_Reconstructed.txt").ReadToEnd();
                //reconstructedText = StringRoutines.Normalize(reconstructedText);

                DiacriticsTester.FindMistakes(originalText, reconstructedText, rPath + text, true);
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

        private static void TestOnTestTexts(DiacriticsReconstructor dr, int rPath)
        {
            foreach (var text in allTexts)
            {
                DiacriticsTester.Test(reconstructorsPath[rPath] + text, dr);
            }
        }

        private static void TestWiki(DiacriticsReconstructor dr)
        {
            for (int i = 7; i < 8; i++)
            {
                var path = wiki_path_AA +"FileDR/"+ wiki_ + string.Format("{0:00}", i);
                //Console.WriteLine(path);
                DiacriticsTester.Test(path, dr);
            }

            DiacriticsTester.PrintOverallStats();
        }

    }
}
