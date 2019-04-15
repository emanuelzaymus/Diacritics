using DiacriticsProject1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xceed.Words.NET;

namespace DiacriticsProject1.Reconstructors
{
    abstract class DiacriticsReconstructor
    {
        abstract protected bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter);

        int[] countOfSolvedWordsByNgrams = new int[5];

        private StringBuilder finalBuilder = new StringBuilder();
        private StringBuilder upCaseStrBuilder = new StringBuilder();
        private StringBuilder wordBuilder = new StringBuilder();

        public string Reconstruct(string text)
        {
            if (text == null)
            {
                return null;
            }

            List<string> parsedStrings = Split(text);
            finalBuilder.Clear();

            string current;
            for (int i = 0; i < parsedStrings.Count; i++)
            {
                if (IsWord(parsedStrings[i]))
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

        private string RecounstructOriginalUpCase(string current, string original)
        {
            upCaseStrBuilder.Clear();
            for (int i = 0; i < original.Length; i++)
            {
                if (Char.IsUpper(original[i]))
                {
                    upCaseStrBuilder.Append(Char.ToUpper(current[i]));
                }
                else
                {
                    upCaseStrBuilder.Append(current[i]);
                }
            }
            return upCaseStrBuilder.ToString();
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

        private bool IsValidChar(char ch)
        {
            return FileCleaner.rgxChars.IsMatch(ch.ToString().ToLower());
        }

        private List<string> Split(string text)
        {
            var parsedStrings = new List<string>();
            bool wasLetter = false;
            bool first = true;

            wordBuilder.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                bool isLetter = IsValidChar(text[i]);
                if (first)
                {
                    wasLetter = isLetter;
                    first = false;
                }
                else if (isLetter && !wasLetter || !isLetter && wasLetter)
                {
                    string str = wordBuilder.ToString();
                    var lowerStr = str.ToLower();
                    if (lowerStr == "www" || lowerStr == "http" || lowerStr == "https" || lowerStr == "ftp" || lowerStr == "ssh")
                    {
                        wordBuilder.Clear();
                        while (i < text.Length && text[i] != ' ' && text[i] != '\n' && text[i] != '\t')
                        {
                            wordBuilder.Append(text[i]);
                            i++;
                        }
                        str += wordBuilder.ToString();
                    }
                    parsedStrings.Add(str);
                    wordBuilder.Clear();
                    wasLetter = isLetter;
                }
                if (i < text.Length)
                {
                    wordBuilder.Append(text[i]);
                }
            }
            parsedStrings.Add(wordBuilder.ToString());

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
                ngramWords[i] = StringRoutines.MyDiacriticsRemover(ngram[i]);
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

        protected void PutToStatistic(string ngram)
        {
            countOfSolvedWordsByNgrams[ngram.Count(x => x == ' ') + 1]++;
        }

        public virtual string GetStatistic()
        {
            StringBuilder stat = new StringBuilder();
            stat.Append("When the words were solved:\n");
            for (int i = countOfSolvedWordsByNgrams.Length - 1; i > 0; i--)
            {
                stat.Append(i);
                stat.Append("-grams: ");
                stat.Append(countOfSolvedWordsByNgrams[i]);
                stat.AppendLine();
            }

            return stat.ToString();
        }

        public virtual void EraseStatistic()
        {
            for (int i = 0; i < countOfSolvedWordsByNgrams.Length; i++)
            {
                countOfSolvedWordsByNgrams[i] = 0;
            }
        }

        public void Reconstruct(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
                throw new Exception("File " + sourcePath + " does not exist!");
            
            string textWithoutDiacritics = File.OpenText(sourcePath).ReadToEnd();

            string reconstructedText = Reconstruct(textWithoutDiacritics);

            File.WriteAllText(destinationPath, reconstructedText);
        }

        public void ReconstructWordDocument(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
                throw new Exception("File " + sourcePath + " does not exist!");
            
            string textWithoutDiacritics;
            using (DocX sourceDoc = DocX.Load(sourcePath))
            {
                textWithoutDiacritics = sourceDoc.Text;
            }

            string reconstructedText = Reconstruct(textWithoutDiacritics);

            using (DocX destinationDoc = DocX.Create(destinationPath))
            {
                destinationDoc.InsertParagraph(reconstructedText);
                destinationDoc.Save();
            }
        }

    }
}
