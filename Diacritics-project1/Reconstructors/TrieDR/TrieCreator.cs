using System;
using System.Collections.Generic;
using System.IO;
using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class TrieCreator
    {
        private Trie<char, List<string>> trie;

        public TrieCreator()
        {
            trie = new Trie<char, List<string>>();
        }

        internal Trie<char, List<string>> Get() => trie;

        internal void Load(List<NgramFile> files)
        {
            foreach (var f in files)
            {
                Load(f);
                Console.WriteLine($"Loaded: {f.FileName}");
            }
        }

        [Flags]
        enum MyEnum : short
        {
            BezDiakritiky = 1,
            SDiakritikou = 2,
            Tretie = 4
        }

        private void temp()
        {
            MyEnum me = MyEnum.SDiakritikou;

            var isFlagBezDiakritiky = (me & MyEnum.BezDiakritiky) == MyEnum.BezDiakritiky;
            
        }

        internal void Load(NgramFile file)
        {
            Ngram ngram;
            string lineWordsFormated;
            while ((ngram = file.Next()) != null)
            {
                lineWordsFormated = string.Join(" ", ngram.Words);
                foreach (string w in ngram.Words)
                {
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

        internal static Trie<char, List<string>> Load(string binaryFilePath, string positionTriePath)
        {
            var ret = new Trie<char, List<string>>();

            using (StreamReader strmReader = File.OpenText(positionTriePath))
            using (var binReader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open)))
            {
                string line;
                int c = 0;
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

                    ret.Add(word, list);
                    if (++c % 10000 == 0) Console.WriteLine(c);
                }
            }

            return ret;
        }

    }
}
