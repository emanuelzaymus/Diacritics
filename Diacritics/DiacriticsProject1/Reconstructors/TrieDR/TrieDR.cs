﻿using System;
using System.Collections.Generic;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.TrieDR
{
    class TrieDR : DiacriticsReconstructor
    {
        private Trie<char, List<string>> trie;

        internal TrieDR(string binaryFilePath, string positionTriePath)
        {
            trie = TrieCreator.Load(binaryFilePath, positionTriePath);
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

        private bool MatchesUp(string word, string ngram, string[] nthBefore, string[] nthAfter, ref string result)
        {
            return base.MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result);
        }

    }
}
