using PBCD.Algorithms.DataStructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileDR : DRBase, IDisposable
    {
        private Trie<char, long> positionTrie;
        private BinaryReader reader;

        public FileDR(string binaryFilePath, string positionTriePath)
        {
            initPositionTrie(positionTriePath);
            reader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open));
        }

        private void initPositionTrie(string path)
        {
            positionTrie = new Trie<char, long>();

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string word = line.Substring(0, line.IndexOf(" "));
                    long position = Convert.ToInt64(line.Substring(line.IndexOf(" ") + 1));

                    positionTrie.Add(word, position);
                }
            }
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            long position = positionTrie.Find(word);

            if (position == 0 && word != "a")
            {
                return false;
            }

            reader.BaseStream.Position = position;
            var length = reader.ReadInt32();

            string ngram;
            string result = null;
            for (int i = 0; i < length; i++)
            {
                ngram = reader.ReadString();
                if (MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result))
                {
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

    }
}
