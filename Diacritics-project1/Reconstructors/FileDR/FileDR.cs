using PBCD.Algorithms.DataStructure;
using System;
using System.IO;
using System.Text;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileDR : DiacriticsReconstructor, IDisposable
    {
        private Trie<char, long> positionTrie;
        private BinaryReader reader;
        private Cache cache;
        private int countOfCacheSolved;

        public FileDR(string binaryFilePath, string positionTriePath)
        {
            positionTrie = PositionTrieCreator.CreatePositionTrie(positionTriePath);
            reader = new BinaryReader(File.OpenRead(binaryFilePath));
            cache = new Cache(1000);
            countOfCacheSolved = 0;
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
                        PutToStatistic(ng);
                        word = result;
                        countOfCacheSolved++;
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

                    PutToStatistic(ngram);
                    word = result;
                    return true;
                }
            }
            throw new Exception("No match in ngrams!");
        }

        //protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        //{
        //    long position = positionTrie.Find(word);

        //    if (position == 0 && word != "a")
        //    {
        //        return false;
        //    }

        //    reader.BaseStream.Position = position;
        //    var length = reader.ReadInt32();
        //    List<string> foundNgrams = new List<string>();

        //    for (int i = 0; i < length; i++)
        //    {
        //        foundNgrams.Add(reader.ReadString());
        //    }
        //    string result = null;
        //    foreach (var ngram in foundNgrams)
        //    {
        //        if (MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result))
        //        {
        //            word = result;
        //            return true;
        //        }
        //    }
        //    throw new Exception("No match in ngrams!");
        //}

        public void Dispose()
        {
            reader.Close();
        }

        public override string GetStatistic()
        {
            return base.GetStatistic() + "From cache: " + countOfCacheSolved + "\n";
        }

        public override void EraseStatistic()
        {
            base.EraseStatistic();
            countOfCacheSolved = 0;
            cache.Clear();
        }

    }
}
