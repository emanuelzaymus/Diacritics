using System;
using System.Collections.Generic;
using System.IO;
using DiacriticsProject1.Reconstructors.FileDR;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.TrieDR
{
    class TrieCreator
    {
        private Trie<char, List<string>> trie;

        public TrieCreator()
        {
            trie = new Trie<char, List<string>>();
        }

        internal Trie<char, List<string>> Get() => trie;

        internal static Trie<char, List<string>> Load(string binaryFilePath, string positionTriePath)
        {
            var ret = new Trie<char, List<string>>();

            using (StreamReader strmReader = File.OpenText(positionTriePath))
            using (var binReader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open)))
            {
                string line;
                while ((line = strmReader.ReadLine()) != null)
                {
                    string word = line.Substring(0, line.IndexOf(" "));
                    long position = Convert.ToInt64(line.Substring(line.IndexOf(" ") + 1));

                    binReader.BaseStream.Position = position;

                    int howMany = binReader.ReadInt32();

                    var list = new List<string>();
                    for (int i = 0; i < howMany; i++)
                    {
                        list.Add(binReader.ReadString());
                    }

                    list = FileCreator.ReduceNumberOfNgrams(list, new int[] { 0, 1, 350, 245, 105 });

                    ret.Add(word, list);
                }
            }
       
            return ret;
        }

    }
}
