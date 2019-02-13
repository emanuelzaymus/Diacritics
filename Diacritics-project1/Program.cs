using Diacritics_project1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Runtime.CompilerServices;
using DiacriticsProject1;

[assembly: InternalsVisibleTo("Diacritics-project1.UnitTests")]


namespace Diacritics_project1
{
    class Program
    {
        static void Main(string[] args)
        {
            //fc.ReadingSpeedTest("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");

            //var fc = new FileCleaner();

            //var file = new UniGramFile(
            //    "D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive.txt");
            //Console.WriteLine(fc.CompleteProcessing(file, rmvWordsFromFreq: 0, rmvBadWordsFromFreq: 11));

            //var file2 = new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-2-gramy.txt");
            //Console.WriteLine(fc.CompleteProcessing(file2, rmvWordsFromFreq: 1));
            //var file3 = new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-3-gramy.txt");
            //Console.WriteLine(fc.CompleteProcessing(file3, rmvWordsFromFreq: 1));
            //var file4 = new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-4-gramy.txt");
            //Console.WriteLine(fc.CompleteProcessing(file4, rmvWordsFromFreq: 1));

            var dr = new DiacriticsReconstructor();

            //DiacriticsTester.Test("D:/testovacie_texty/1/Retz.txt", dr);
            DiacriticsTester.Test("D:/testovacie_texty/2/Ako sa eseťákom pokúsili ukradnúť hotel Carlton.txt", dr);
            DiacriticsTester.Test("D:/testovacie_texty/3/Macron ako novodobý Ľudovít XVI.txt", dr);
            DiacriticsTester.Test("D:/testovacie_texty/4/Bobby Robson.txt", dr);
            DiacriticsTester.Test("D:/testovacie_texty/5/Mor ho.txt", dr);
            DiacriticsTester.Test("D:/testovacie_texty/6/PETER HOTRA.txt", dr);
            

        }

    }
}
