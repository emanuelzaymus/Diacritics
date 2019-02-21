using System;
using System.Collections.Generic;
using DiacriticsProject1.Common.Files;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class TrieDR : DRBase
    {
        private Trie<char, List<string>> trie;

        public TrieDR()
        {
            var files = new List<NgramFile> {
                //new NgramFile("D:/ngramy/prim-8.0-public-all-4-gramy/prim-8.0-public-all-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-3-gramy/prim-8.0-public-all-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                //new NgramFile("D:/ngramy/prim-8.0-public-all-2-gramy/prim-8.0-public-all-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt")
            };

            var creator = new TrieCreator();

            foreach (var f in files)
            {
                creator.Load(f);
                Console.WriteLine($"Loaded: {f.FileName}");
            }

            trie = creator.Get();
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            List<string> foundNgrams;
            if ((foundNgrams = trie.Find(word)) != null)
            {
                string result = null;
                foreach (var ngram in foundNgrams)
                {
                    if (MatchesUp(word, ngram, nthBefore, nthAfter, ref result))
                    {
                        word = result;
                        return true;
                    }
                }
                throw new Exception("No match in ngrams!");
            }
            return false;
        }

    }
}
