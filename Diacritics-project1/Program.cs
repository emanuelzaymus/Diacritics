using Diacritisc_project1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Diacritics_project1
{
    class Program
    {
        static void Main(string[] args)
        {
            //fc.ReadingSpeedTest("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");

            //var fc = new FileCleaner();

            //var file = new NgramFile(
            //    "D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive.txt",
            //    NgramFile.Type.Dictionary);
            //Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11));

            //var file2 = new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-4-gramy.txt", NgramFile.Type.Ngrams);
            //Console.WriteLine(fc.CompleteProcessing(file2, rmvWordsFromFreq: 1));

            DiacriticsTester.Test("E:/testovacie_texty/Retz.txt");

            return;

        }

    }
}
