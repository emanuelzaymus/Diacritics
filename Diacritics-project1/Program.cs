using System;
using System.IO;
using System.Text;

namespace Diacritics_project1
{
    class Program
    {
        static void Main(string[] args)
        {
            //fc.ReadingSpeedTest("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");

            var fc = new FileCleaner();

            Console.WriteLine(fc.CompleteProcessing("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive.txt",
                FileCleaner.Type.Dictionary, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11));

            Console.WriteLine(fc.CompleteProcessing("D:/ngramy/prim-8.0-public-img-sk-2-gramy.txt", FileCleaner.Type.Ngrams));

        }
    }
}
