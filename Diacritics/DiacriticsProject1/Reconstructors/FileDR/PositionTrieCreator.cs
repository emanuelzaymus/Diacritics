using PBCD.Algorithms.DataStructure;
using System;
using System.IO;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class PositionTrieCreator
    {

        internal static Trie<char, long> CreatePositionTrie(string path)
        {
            var retPositionTrie = new Trie<char, long>();

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string word = line.Substring(0, line.IndexOf(" "));
                    long position = Convert.ToInt64(line.Substring(line.IndexOf(" ") + 1));

                    retPositionTrie.Add(word, position);
                }
            }
            return retPositionTrie;
        }

    }
}
