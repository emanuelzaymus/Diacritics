using Diacritisc_project1;
using System;

namespace Diacritics_project1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fc = new FileCleaner();

            //fc.ReadingSpeedTest("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");

            //string name = fc.CompleteProcessing("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive.txt",
            //    removeFromFreq: 0, badWordsFrmoFreq: 11);
            //Console.WriteLine(name);

            //string name2 = fc.CompleteProcessing("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");
            //Console.WriteLine(name2);

            var ngfc = new NgramsFileCleaner();

            //ngfc.Clean("D:/ngramy/prim-7.0-frk-4-gramy.txt");
            ngfc.Clean("D:/ngramy/prim-7.0-frk-2-gramy.txt");


        }
    }
}
