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
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-4-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-3-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                new NgramFile("D:/ngramy/prim-8.0-public-img-sk-n-gramy/prim-8.0-public-img-sk-2-gramy_TO-1_CLEANED_GOOD-WORDS.txt"),
                new UniGramFile("D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt")
            };

            var creator = new TrieCreator();

            foreach (var f in files)
            {
                creator.Load(f);
                Console.WriteLine($"Loaded: {f.FileName()}");
            }

            trie = creator.Get();
        }

        internal string Reconstruct(string text)
        {
            List<string> parsedStrings = split(text);
            StringBuilder finalBuilder = new StringBuilder();

            string current;
            for (int i = 0; i < parsedStrings.Count; i++)
            {
                if (isWord(parsedStrings[i]) && !isURL(parsedStrings[i]))
                {
                    nearWords(parsedStrings, i, out string[] nthBefore, out string[] nthAfter);
                    current = normalize(parsedStrings[i]);
                    if (setDiacritics(ref current, nthBefore, nthAfter))
                    {
                        current = recounstructOriginalUpCase(current, parsedStrings[i]);
                        finalBuilder.Append(current);
                    }
                    else
                    {
                        finalBuilder.Append(parsedStrings[i]);
                    }
                }
                else
                {
                    finalBuilder.Append(parsedStrings[i]);
                }
            }

            return finalBuilder.ToString();
        }

        protected bool isURL(string str)
        {
            string[] domains = { "http", ".sk", ".com", ".cz", ".uk", ".us", ".to", ".org", ".pl",
                ".de", ".net", ".gov", ".edu", ".ru", ".fr", ".es", ".ch", ".ca", ".at", ".info" };
            foreach (var dom in domains)
            {
                if (str.Contains(dom))
                {
                    return true;
                }
            }
            return false;
        }

        private string recounstructOriginalUpCase(string current, string original)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < original.Length; i++)
            {
                if (Char.IsUpper(original[i]))
                {
                    sb.Append(Char.ToUpper(current[i]));
                }
                else
                {
                    sb.Append(current[i]);
                }
            }
            return sb.ToString();
        }

        private void nearWords(List<string> parsedStrings, int wordPosition, out string[] nthBefore, out string[] nthAfter)
        {
            nthBefore = new string[3];
            nthAfter = new string[3];

            for (int i = 0; i < nthBefore.Length; i++)
            {
                if ((wordPosition - 2 * i - 2) >= 0)
                {
                    nthBefore[i] = parsedStrings[wordPosition - 2 * i - 2];
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < nthAfter.Length; i++)
            {
                if ((wordPosition + 2 * i + 2) < parsedStrings.Count)
                {
                    nthAfter[i] = parsedStrings[wordPosition + 2 * i + 2];
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
            string[] ngramWordsDiacritics = ngram.Split(' ');
            ngram = FileCleaner.RemoveDiacritics(ngram);
            string[] ngramWords = ngram.Split(' ');
            bool matches;
            int res;

            for (int i = 0; i < ngramWords.Length; i++)
            {
                // find {word} in {ngramWords} (multiple matches can by found)
                if (ngramWords[i] == word)
                {
                    res = i;
                    matches = true;
                    // test {ngramWords} with {nthBefore} and {nthAfter}
                    for (int j = 0; j < nthBefore.Length; j++)
                    {
                        if ((i - j - 1) >= 0)
                        {
                            if (nthBefore[j] != ngramWords[i - j - 1])
                            {
                                matches = false;
                                break;
                            }
                        }
                        else break;
                    }

                    if (matches)
                    {
                        for (int j = 0; j < nthAfter.Length; j++)
                        {
                            if ((i + j + 1) < ngramWords.Length)
                            {
                                if (nthAfter[j] != ngramWords[i + j + 1])
                                {
                                    matches = false;
                                    break;
                                }
                            }
                            else break;
                        }
                    }

                    if (matches)
                    {
                        result = ngramWordsDiacritics[res];
                        return true;
                    }
                }
            }
            return false;
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
