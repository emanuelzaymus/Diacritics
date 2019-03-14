using PBCD.Algorithms.DataStructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileDR : DRBase, IDisposable
    {
        private Trie<char, int> positionTrie;
        private BinaryReader reader;

        public FileDR()
        {
            initPositionTrie("D:/binFile/fileTrie.txt");
            reader = new BinaryReader(File.Open("D:/binFile/data.dat", FileMode.Open));
        }

        private void initPositionTrie(string path)
        {
            positionTrie = new Trie<char, int>();

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string word = line.Substring(0, line.IndexOf(" "));
                    int position = Convert.ToInt32(line.Substring(line.IndexOf(" ") + 1));

                    positionTrie.Add(word, position);
                }
            }
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            int position = positionTrie.Find(word);

            if (position == 0 && word != "a")
            {
                return false;
            }

            reader.BaseStream.Position = position;
            var length = reader.ReadInt32();
            List<string> foundNgrams = new List<string>();

            for (int i = 0; i < length; i++)
            {
                foundNgrams.Add(reader.ReadString());
            }
            string result = null;
            foreach (var ngram in foundNgrams)
            {
                if (MatchesUp(word, ngram.Split(' '), nthBefore, nthAfter, ref result))
                {
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
