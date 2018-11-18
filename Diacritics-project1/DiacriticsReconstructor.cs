using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBCD.Algorithms.DataStructure;
using Diacritics_project1;
using System.Text.RegularExpressions;

namespace Diacritisc_project1
{
    class DiacriticsReconstructor
    {
        private Trie<char, List<string>> trie;

        public DiacriticsReconstructor()
        {
            var files = new List<NgramFile> {
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt", NgramFile.Type.Ngrams),
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt", NgramFile.Type.Ngrams),
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt", NgramFile.Type.Ngrams),
                new NgramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt", NgramFile.Type.Dictionary)
            };

            var creator = new TrieCreator();

            foreach (var f in files)
            {
                creator.Load(f);
            }

            trie = creator.Get();
        }

        internal string Reconstruct(string text)
        {
            List<string> parsedStrings = split(text);
            StringBuilder finalBuilder = new StringBuilder();

            string current;
            for (int i = 0; i < parsedStrings.Capacity; i++)
            {
                if (isWord(parsedStrings[i]))
                {
                    nearWords(parsedStrings, i, out string[] nthBefore, out string[] nthAfter);
                    current = normalize(parsedStrings[i]);
                    finalBuilder.Append(setDiacritics(ref current, nthBefore, nthAfter) ? current : parsedStrings[i]);
                }
                else
                {
                    finalBuilder.Append(parsedStrings[i]);
                }
            }

            return finalBuilder.ToString();
        }

        private void nearWords(List<string> parsedStrings, int i, out string[] nthBefore, out string[] nthAfter)
        {
            nthBefore = new string[3];
            nthAfter = new string[3];

            for (int j = 0; j < nthBefore.Length; j++)
            {
                if (i - 2 * j - 2 >= 0)
                {
                    nthBefore[j] = parsedStrings[i - 2 * j - 2];
                }
                else
                {
                    break;
                }
            }

            for (int j = 0; j < nthAfter.Length; j++)
            {
                if (i + 2 * j + 2 < parsedStrings.Capacity)
                {
                    nthAfter[j] = parsedStrings[i + 2 * j + 2];
                }
                else
                {
                    break;
                }
            }
        }

        private bool setDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            List<string> foundNgrams;
            if ((foundNgrams = trie.Find(word)) != null)
            {
                string result = null;
                foreach (var ngram in foundNgrams)
                {
                    if (matchesUp(word, ngram, nthBefore, nthAfter, ref result))
                    {
                        word = result;
                        return true;
                    }

                }
                throw new Exception("No match in ngrams!");
            }
            return false;
        }

        private bool matchesUp(string word, string ngram, string[] nthBefore, string[] nthAfter, ref string result)
        {
            ngram = FileCleaner.RemoveDiacritics(ngram);
            string[] ngramWords = ngram.Split(' ');

            // find {word} in {ngramWords} (multiple matches can by found)
            // test {ngramWords} with {nthBefore} and {nthAfter}


            throw new NotImplementedException();
        }

        private bool isWord(string str)
        {
            return Regex.IsMatch(str, FileCleaner.charsPattern);
        }

        private List<string> split(string text)
        {
            var parsedStrings = new List<string>();
            bool wasLetter = false;
            bool first = true;

            StringBuilder wordBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                bool isLetter = Regex.IsMatch(text[i].ToString().ToLower(), FileCleaner.charsPattern);
                if (first)
                {
                    wasLetter = isLetter;
                    first = false;
                }
                else if (isLetter && !wasLetter || !isLetter && wasLetter)
                {
                    parsedStrings.Add(wordBuilder.ToString());
                    wordBuilder = new StringBuilder();
                    wasLetter = isLetter;
                }
                wordBuilder.Append(text[i]);
            }

            return parsedStrings;
        }

        private string normalize(string word)
        {
            return FileCleaner.RemoveDiacritics(word).ToLower();
        }

    }
}
