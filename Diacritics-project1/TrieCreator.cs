using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diacritics_project1;
using System.IO;
using PBCD.Algorithms.DataStructure;

namespace Diacritisc_project1
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
            int i = 10;
            while ((ngram = file.Next()) != null)
            {
                foreach (string w in ngram.Words)
                {
                    string nonDiacriticsWord = FileCleaner.RemoveDiacritics(w);
                    List<string> foundList = trie.Find(nonDiacriticsWord);
                    if (foundList == null)
                    {
                        trie.Add(nonDiacriticsWord, new List<string> { $"{ngram.Frequency} - {w}" });
                    }
                    else
                    {
                        foundList.Add($"{ngram.Frequency} - {w}");
                    }
                }
                if (--i <= 0) return;
            }
        }

    }
}
