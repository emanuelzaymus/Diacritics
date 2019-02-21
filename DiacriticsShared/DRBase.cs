using System;
using System.Collections.Generic;
using System.Text;

namespace DiacriticsShared
{
    abstract class DRBase : IDiacriticsReconstructor
    {
        abstract public string Reconstruct(string text);
        //protected bool MatchesUp(string word, string ngram, string[] nthBefore, string[] nthAfter, ref string result)
        //{
        //    string[] ngramWordsDiacritics = ngram.Split(' ');
        //    ngram = StringRoutines.MyDiacriticsRemover(ngram);
        //    string[] ngramWords = ngram.Split(' ');
        //    bool matches;
        //    int res;

        //    for (int i = 0; i < ngramWords.Length; i++)
        //    {
        //        // find {word} in {ngramWords} (multiple matches can by found)
        //        if (ngramWords[i] == word)
        //        {
        //            res = i;
        //            matches = true;
        //            // test {ngramWords} with {nthBefore} and {nthAfter}
        //            for (int j = 0; j < nthBefore.Length; j++)
        //            {
        //                if ((i - j - 1) >= 0)
        //                {
        //                    if (nthBefore[j] != ngramWords[i - j - 1])
        //                    {
        //                        matches = false;
        //                        break;
        //                    }
        //                }
        //                else break;
        //            }

        //            if (matches)
        //            {
        //                for (int j = 0; j < nthAfter.Length; j++)
        //                {
        //                    if ((i + j + 1) < ngramWords.Length)
        //                    {
        //                        if (nthAfter[j] != ngramWords[i + j + 1])
        //                        {
        //                            matches = false;
        //                            break;
        //                        }
        //                    }
        //                    else break;
        //                }
        //            }

        //            if (matches)
        //            {
        //                result = ngramWordsDiacritics[res];
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //protected bool IsWord(string str)
        //{
        //    return FileCleaner.rgxChars.IsMatch(str);
        //}
        //protected List<string> Split(string text)
        //{
        //    // TODO: html www ftp ignorovat
        //    var parsedStrings = new List<string>();
        //    bool wasLetter = false;
        //    bool first = true;

        //    StringBuilder wordBuilder = new StringBuilder();
        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        bool isLetter = FileCleaner.rgxChars.IsMatch(text[i].ToString().ToLower());
        //        if (first)
        //        {
        //            wasLetter = isLetter;
        //            first = false;
        //        }
        //        else if (isLetter && !wasLetter || !isLetter && wasLetter)
        //        {
        //            parsedStrings.Add(wordBuilder.ToString());
        //            wordBuilder.Clear(); // = new StringBuilder(); // todo untested
        //            wasLetter = isLetter;
        //        }
        //        wordBuilder.Append(text[i]);
        //    }

        //    return parsedStrings;
        //}
        //protected string Normalize(string word)
        //{
        //    return StringRoutines.MyDiacriticsRemover(word).ToLower();
        //}
    }
}
