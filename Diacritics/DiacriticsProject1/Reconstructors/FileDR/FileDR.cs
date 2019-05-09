using PBCD.Algorithms.DataStructure;
using System;
using System.IO;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileDR : DiacriticsReconstructor, IDisposable
    {
        private static Trie<char, long> positionTrie;
        private BinaryReader reader;
        private Cache cache;

        internal FileDR(string binaryFilePath, string positionTriePath)
        {
            if (positionTrie == null)
            {
                positionTrie = PositionTrieCreator.CreatePositionTrie(positionTriePath);
            }
            reader = new BinaryReader(File.OpenRead(binaryFilePath));
            cache = new Cache(1000);
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            long position = positionTrie.Find(word);

            if (position == 0 && word != "a")
            {
                return false;
            }
            string result = null;

            var cacheList = cache.Get(word);
            if (cacheList != null)
            {
                foreach (var ng in cacheList)
                {
                    if (MatchesUp(word, ng.Split(' '), nthBefore, nthAfter, ref result))
                    {
                        word = result;
                        return true;
                    }
                }
            }

            reader.BaseStream.Position = position;
            var length = reader.ReadInt32();

            string ngram;
            for (int i = 0; i < length; i++)
            {
                ngram = reader.ReadString();
                if (MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result))
                {
                    cache.Add(ngram);

                    word = result;
                    return true;
                }
            }
            throw new Exception("No match in ngrams!");
        }

        public void Dispose()
        {
            reader.Close();
        }

    }
}
