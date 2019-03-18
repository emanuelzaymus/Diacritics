using DiacriticsProject1.Common.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileStatistics
    {

        internal static string BinaryFileNgramStats(string binFilePath, string positionTriePath)
        {
            string statName = TextFile.FileName(binFilePath) + "_STATS.txt";

            using (StreamReader positionReader = File.OpenText(positionTriePath))
            using (var binaryReader = new BinaryReader(File.Open(binFilePath, FileMode.Open)))
            using (var statWriter = new StreamWriter(statName))
            {
                int[] allNgCount = new int[5];
                string line;
                while ((line = positionReader.ReadLine()) != null)
                {
                    string word = line.Substring(0, line.IndexOf(" "));
                    long position = Convert.ToInt64(line.Substring(line.IndexOf(" ") + 1));

                    int[] ngCount = new int[5];

                    binaryReader.BaseStream.Position = position;
                    var len = binaryReader.ReadInt32();
                    for (int i = 0; i < len; i++)
                    {
                        string ng = binaryReader.ReadString();
                        string[] ngArr = ng.Split(' ');
                        int length = ngArr.Length;
                        ngCount[length]++;
                    }
                    statWriter.WriteLine($"{word} ({ngCount.Sum()}) ({ngCount[4]}, {ngCount[3]}, {ngCount[2]}, {ngCount[1]})");

                    for (int i = 1; i < 5; i++) { allNgCount[i] += ngCount[i]; }
                }
                statWriter.WriteLine("All ngrams: {0} (4: {1}, 3: {2}, 2: {3}, 1: {4})",
                    allNgCount.Sum(), allNgCount[4], allNgCount[3], allNgCount[2], allNgCount[1]);
            }

            return statName;
        }

        public static string BinaryFilePartitioningStats(string binFilePath, string positionTriePath, string word)
        {
            string statName = TextFile.FileName(binFilePath) + "PARTITION-STATS-" + word + ".txt";

            var positionTrie = PositionTrieCreator.CreatePositionTrie(positionTriePath);

            using (var binaryReader = new BinaryReader(File.Open(binFilePath, FileMode.Open)))
            using (var statWriter = new StreamWriter(statName))
            {
                var position = positionTrie.Find(word);

                binaryReader.BaseStream.Position = position;
                int howMany = binaryReader.ReadInt32();
                var ngrams = new List<string>();

                for (int i = 0; i < howMany; i++)
                {
                    ngrams.Add(binaryReader.ReadString());
                }

                statWriter.WriteLine($"Word: {word}");
                int len = 1;
                while (ngrams.Count != 0)
                {
                    var found = ngrams.Where(x => x.Length == len);

                    if (found.Count() != 0)
                    {
                        statWriter.WriteLine($"{len} - {found.Count()}");

                        foreach (var n in found.ToList())

                        {
                            ngrams.Remove(n);
                        }
                    }
                    len++;
                }
            }
            return statName;
        }

    }
}
