using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class NgramFilesCreator
    {
        private static string rootFolder;
        private static readonly string fileExtension = ".txt";
        public static void Create(string rootFolder)
        {
            NgramFilesCreator.rootFolder = rootFolder;

            var files = new List<NgramFile> {
                //new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt")
                new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/skuska.txt")
            };

            foreach (var f in files)
            {
                WorkIn(f);
                Console.WriteLine($"Worked in: {f.FileName}");
            }
        }

        private static void WorkIn(NgramFile file)
        {
            Ngram ngram;
            string lineWordsFormated;
            while ((ngram = file.Next()) != null)
            {
                lineWordsFormated = string.Join(" ", ngram.Words);
                foreach (string w in ngram.Words)
                {
                    string nonDiacriticsWord = StringRoutines.MyDiacriticsRemover(w);
                    File.AppendAllText($"{rootFolder}/{nonDiacriticsWord}{fileExtension}", lineWordsFormated + "\n");
                }
            }
        }

    }
}
