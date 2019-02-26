using DiacriticsProject1.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiacriticsProject1.Reconstructors
{
    abstract class DRBase : IDiacriticsReconstructor
    {
        abstract protected bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter);

        public string Reconstruct(string text)
        {
            List<string> parsedStrings = Split(text);
            StringBuilder finalBuilder = new StringBuilder();

            string current;
            for (int i = 0; i < parsedStrings.Count; i++)
            {
                if (IsWord(parsedStrings[i]) /*&& !IsURL(parsedStrings[i])*/)
                {
                    NearWords(parsedStrings, i, out string[] nthBefore, out string[] nthAfter);
                    current = Normalize(parsedStrings[i]);
                    if (SetDiacritics(ref current, nthBefore, nthAfter))
                    {
                        current = RecounstructOriginalUpCase(current, parsedStrings[i]);
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

        private bool IsURL(string str) // TODO: doesnt work ({this.split} splits the whole ulr apart)
        {
            string[] domains = { "http", "www", "ftp", ".sk", ".com", ".cz", ".uk", ".us", ".to", ".org", ".pl",
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

        private string RecounstructOriginalUpCase(string current, string original)
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

        private void NearWords(List<string> parsedStrings, int wordPosition, out string[] nthBefore, out string[] nthAfter)
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

        private bool IsWord(string str)
        {
            return FileCleaner.rgxChars.IsMatch(str);
        }
        private List<string> Split(string text)
        {
            // TODO: html www ftp ignorovat
            var parsedStrings = new List<string>();
            bool wasLetter = false;
            bool first = true;

            StringBuilder wordBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                bool isLetter = FileCleaner.rgxChars.IsMatch(text[i].ToString().ToLower());
                if (first)
                {
                    wasLetter = isLetter;
                    first = false;
                }
                else if (isLetter && !wasLetter || !isLetter && wasLetter)
                {
                    parsedStrings.Add(wordBuilder.ToString());
                    wordBuilder.Clear(); // = new StringBuilder(); // todo untested
                    wasLetter = isLetter;
                }
                wordBuilder.Append(text[i]);
            }

            return parsedStrings;
        }
        private string Normalize(string word)
        {
            return StringRoutines.MyDiacriticsRemover(word).ToLower();
        }

        protected bool MatchesUp(string word, string[] ngram, string[] nthBefore, string[] nthAfter, ref string result)
        {
            string[] ngramWordsDiacritics = ngram;
            string[] ngramWords = new string[ngram.Length];
            for (int i = 0; i < ngram.Length; i++)
            {
                ngramWords[i]= StringRoutines.MyDiacriticsRemover(ngram[i]);
            }

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

    }
}
