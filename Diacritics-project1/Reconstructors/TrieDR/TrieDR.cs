﻿using System;
using System.Collections.Generic;
using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class TrieDR : DRBase
    {
        private Trie<char, List<string>> trie;

        public TrieDR(List<NgramFile> files)
        {
            var creator = new TrieCreator();

            creator.Load(files);

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

        private bool MatchesUp(string word, string ngram, string[] nthBefore, string[] nthAfter, ref string result)
        {
            return base.MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result);
        }

    }
}