using DiacriticsProject1.Common;
using PBCD.Algorithms.DataStructure;
using System.Collections.Generic;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class Cache
    {

        private Trie<char, List<string>> cache = new Trie<char, List<string>>();

        private int size;
        private bool isSetSize;
        private List<string> priorityNgrams; // Last n (size) used ngrams.

        public Cache()
        {
            isSetSize = false;
        }

        public Cache(int size)
        {
            this.size = size;
            isSetSize = true;
            priorityNgrams = new List<string>();
        }

        public void Add(string ngram)
        {
            if (ngram == null)
            {
                return;
            }
            var nonDiacritics = StringRoutines.MyDiacriticsRemover(ngram);

            foreach (var word in nonDiacritics.Split(' '))
            {
                var ngrams = cache.Find(word);
                if (ngrams != null)
                {
                    ngrams.Add(ngram);
                }
                else
                {
                    cache.Add(word, new List<string> { ngram });
                }
            }

            if (isSetSize)
            {
                CheckSize(ngram);
            }
        }

        private void CheckSize(string ngram)
        {
            priorityNgrams.Remove(ngram);
            priorityNgrams.Add(ngram);
            if (priorityNgrams.Count > size)
            {
                var ngramToRemove = StringRoutines.MyDiacriticsRemover(priorityNgrams[0]);
                foreach (var word in ngramToRemove.Split(' '))
                {
                    var listRemoveFrom = cache.Find(word);
                    if (listRemoveFrom == null)
                    {
                        continue;
                    }
                    listRemoveFrom.Remove(priorityNgrams[0]);
                    if (listRemoveFrom.Count == 0)
                    {
                        cache.Remove(word);
                    }
                }
                priorityNgrams.RemoveAt(0);
            }
        }

        internal List<string> Get(string key)
        {
            if (key == null)
            {
                return null;
            }
            return cache.Find(key);
        }

        public void Clear()
        {
            cache = new Trie<char, List<string>>();
        }

    }
}
