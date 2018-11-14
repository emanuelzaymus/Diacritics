using System;

namespace Diacritics_project1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fc = new FileCleaner();

            //fc.ReadingSpeedTest("D:/slovniky/prim-8.0-public-sane-word_frequency_non_case_sensitive.txt");

            //fc.Clean("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive.txt");

            fc.RemoveBadWords("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive_CLEANED.txt");

            //fc.RemoveWordsWithFreq("D:/slovniky/prim-8.0-public-img-sk-word_frequency_non_case_sensitive_CLEANED.txt", 1);

        }
    }
}
