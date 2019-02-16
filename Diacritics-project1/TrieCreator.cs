﻿using System.Collections.Generic;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1
{
    class TrieCreator
    {
        private Trie<char, List<string>> trie;

        public TrieCreator()
        {
            trie = new Trie<char, List<string>>();
        }

        internal Trie<char, List<string>> Get() => trie;

        internal void Load(NgramFile file)
        {
            Ngram ngram;
            string lineWordsFormated;
            while ((ngram = file.Next()) != null)
            {
                lineWordsFormated = string.Join(" ", ngram.Words);
                foreach (string w in ngram.Words)
                {
                    //string nonDiacriticsWord = FileCleaner.RemoveDiacritics(w);
                    string nonDiacriticsWord = StringRoutines.MyDiacriticsRemover(w);
                    List<string> foundList = trie.Find(nonDiacriticsWord);
                    if (foundList == null)
                    {
                        trie.Add(nonDiacriticsWord, new List<string> { lineWordsFormated });
                    }
                    else
                    {
                        foundList.Add(lineWordsFormated);
                    }
                }
            }
        }

    }
}
