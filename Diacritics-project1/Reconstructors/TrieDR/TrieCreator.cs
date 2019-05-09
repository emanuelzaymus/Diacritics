using System;
using System.Collections.Generic;
using System.IO;
using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
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

        internal void Load(List<NgramFile> files)
        {
            foreach (var f in files)
            {
                Load(f);
                Console.WriteLine($"Loaded: {f.FileName}");
            }
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

        internal void GetOptimizedTrie(UniGramFile uniGramFiles, List<NgramFile> otherNgramFiles)
        {
            var ngramLists = new List<List<string>>();
            LoadUnigrams(uniGramFiles, ngramLists);
            Console.WriteLine("unigrams loaded");
            OptimizeUniGramTrie(ngramLists);
            Console.WriteLine("unigrams optimized");
            OptimizedLoad(otherNgramFiles);
            Console.WriteLine("other files loaded");
            SwitchUniGramsToTheEnd(ngramLists);
            Console.WriteLine("unigrams switched");
        }

        private void LoadUnigrams(UniGramFile uniGramFiles, List<List<string>> ngramLists)
        {
            Ngram ngram;
            string lineWordsFormated;
            while ((ngram = uniGramFiles.Next()) != null)
            {
                lineWordsFormated = string.Join(" ", ngram.Words);
                foreach (string w in ngram.Words)
                {
                    string nonDiacriticsWord = StringRoutines.MyDiacriticsRemover(w);
                    List<string> foundList = trie.Find(nonDiacriticsWord);
                    if (foundList == null)
                    {
                        var l = new List<string> { lineWordsFormated };
                        ngramLists.Add(l);
                        trie.Add(nonDiacriticsWord, l);
                    }
                    else
                    {
                        foundList.Add(lineWordsFormated);
                    }
                }
            }
        }

        private void OptimizeUniGramTrie(List<List<string>> ngramLists)
        {
            for (int i = 0; i < ngramLists.Count; i++)
            {
                var l = ngramLists[i];
                if (l.Count == 1)
                {
                    if (!FileCleaner.rgxNonLatinChars.IsMatch(l[0]))
                    {
                        trie.Remove(l[0]);
                        l = null;
                    }
                }
                else if (l.Count > 2)
                {
                    l.RemoveRange(2, l.Count - 2);
                }
            }
            ngramLists.RemoveAll(x => x == null);
        }

        private void OptimizedLoad(List<NgramFile> otherNgramFiles)
        {
            var maxAllowedConut = new int[5] { 0, 702, 702, 352, 107 };
            // load 1, 350, 245, 105  (1 2 3 4)
            foreach (var file in otherNgramFiles)
            {
                Ngram ngram;
                string lineWordsFormated;

                int size = file.Next().Words.Length;
                file.ReOpen();

                while ((ngram = file.Next()) != null)
                {
                    lineWordsFormated = string.Join(" ", ngram.Words);
                    foreach (string w in ngram.Words)
                    {
                        string nonDiacriticsWord = StringRoutines.MyDiacriticsRemover(w);
                        List<string> foundList = trie.Find(nonDiacriticsWord);

                        if (foundList != null && foundList.Count > 1 && foundList.Count < maxAllowedConut[size])
                        {
                            foundList.Add(lineWordsFormated);
                        }
                    }
                }
                Console.WriteLine(" 4 3 2");
            }
        }

        private void SwitchUniGramsToTheEnd(List<List<string>> ngramLists)
        {
            for (int i = 0; i < ngramLists.Count; i++)
            {
                var l = ngramLists[i];
                if (l.Count > 1)
                {
                    string uniGram = l[0];
                    l.RemoveAt(0);
                    l.Add(uniGram);

                    if (!l[0].Contains(" "))
                    {
                        l.RemoveAt(0);
                    }
                }
            }
        }

        internal static Trie<char, List<string>> Load(string binaryFilePath, string positionTriePath)
        {
            var ret = new Trie<char, List<string>>();

            using (StreamReader strmReader = File.OpenText(positionTriePath))
            using (var binReader = new BinaryReader(File.OpenRead(binaryFilePath)))
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

                    list = FileCreator.ReduceNumberOfNgrams(list, new int[] { 0, 1, 350, 245, 105 });

                    ret.Add(word, list);
                    if (++c % 10000 == 0) Console.WriteLine(c);
                }
            }

            return ret;
        }

    }
}
