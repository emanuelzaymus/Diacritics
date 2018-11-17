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

            //var file = new NgramFile("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt",
            //    NgramFile.Type.Dictionary);
            //Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11));

            //var file2 = new NgramFile("D:/ngramy/prim-8.0-public-img-sk-2-gramy.txt", NgramFile.Type.Ngrams);
            //Console.WriteLine(fc.CompleteProcessing(file2));

            var file3 = new NgramFile("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt",
                NgramFile.Type.Dictionary);

            var creator = new TrieCreator();

            creator.Load(file3);

            var t = creator.Get();


            
            Console.WriteLine(t.Find("a")[0]);
            Console.WriteLine(t.Find("v")[0]);
            Console.WriteLine(t.Find("sa")[0]);
            Console.WriteLine(t.Find("na")[0]);
            Console.WriteLine(t.Find("je")[0]);
            Console.WriteLine(t.Find("ze")[0]);
            Console.WriteLine(t.Find("s")[0]);
            Console.WriteLine(t.Find("z")[0]);
            Console.WriteLine(t.Find("to")[0]);
            Console.WriteLine(t.Find("aj")[0]);
            Console.WriteLine(t.Find("o"));


        }
    }
}
