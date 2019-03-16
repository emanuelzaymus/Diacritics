using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using DiacriticsProject1.Reconstructors.DBDR;
using PBCD.Algorithms.DataStructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileCreator
    {
        private Trie<char, int> idTrie;
        private int maxId;
        private int minId;

        private void initAttrs()
        {
            using (var db = new DiacriticsDBEntities())
            {
                idTrie = DBTrieCreator.CreateDBTrie(db);
                maxId = db.Words.Max(x => x.Id);
                minId = db.Words.Min(x => x.Id);
            }
        }

        internal static string CreateStatisticsAboutBinaryFile(string binFilePath, string positionTriePath)
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

        internal void CreateCompoundBinaryFile(string[] binFiles, string binFilePath, string positionTriePath)
        {
            var words = GetWords();
            Console.WriteLine("words are loaded");
            var allBinFiles = GetAllPartialBinFiles(binFiles);
            var binaryReaders = GetBinaryReaders(allBinFiles);
            Console.WriteLine("all binary readers are ready");

            using (var trieWriter = new StreamWriter(positionTriePath))
            using (var binaryWriter = new BinaryWriter(File.Open(binFilePath, FileMode.Create)))
            {
                int c = 0;
                foreach (var w in words)
                {
                    trieWriter.WriteLine(w + " " + binaryWriter.BaseStream.Position);

                    var allRelevantNgrams = new List<string>();
                    foreach (var reader in binaryReaders)
                    {
                        int howMany = reader.ReadInt32();
                        for (int i = 0; i < howMany; i++)
                        {
                            allRelevantNgrams.Add(reader.ReadString());
                        }
                    }
                    binaryWriter.Write(allRelevantNgrams.Count);
                    foreach (var ng in allRelevantNgrams)
                    {
                        binaryWriter.Write(ng);
                    }
                    if (++c % 100000 == 0) { Console.WriteLine(c); }
                }
            }

            Close(binaryReaders);
        }

        private List<string> GetWords()
        {
            var words = new List<string>();

            using (var db = new DiacriticsDBEntities())
            {
                db.Database.Connection.Open();
                var sqlSelectWord = new SqlCommand("SELECT Value FROM dbo.Words ORDER BY Id ASC",
                        db.Database.Connection as SqlConnection);
                sqlSelectWord.CommandType = CommandType.Text;

                using (SqlDataReader wordReader = sqlSelectWord.ExecuteReader())
                {
                    while (wordReader.Read())
                    {
                        words.Add(wordReader.GetString(0));
                    }
                }
                db.Database.Connection.Close();
            }
            return words;
        }

        private void Close(List<BinaryReader> binaryReaders)
        {
            foreach (var reader in binaryReaders)
            {
                reader.Close();
            }
        }

        private List<BinaryReader> GetBinaryReaders(List<string> allBinFiles)
        {
            var ret = new List<BinaryReader>();

            foreach (var path in allBinFiles)
            {
                ret.Add(new BinaryReader(File.Open(path, FileMode.Open)));
            }

            return ret;
        }

        private List<string> GetAllPartialBinFiles(string[] binFiles)
        {
            var ret = new List<string>();

            int size = 4;
            foreach (var path in binFiles)
            {
                int stepSize = GetDivisionCountByNumber(size--);
                int i = 0;

                string finalPath;
                while (File.Exists(finalPath = TextFile.FileName(path) + i + TextFile.FileExtension(path)))
                {
                    ret.Add(finalPath);
                    Console.WriteLine(finalPath);
                    i += stepSize;
                }
            }

            return ret;
        }

        // Creating partial files

        internal void CreatePartialBinaryFilesFromFiles(List<NgramFile> files, string directoryPath)
        {
            initAttrs();

            foreach (var f in files)
            {
                int count = GetSuitableCountForDivision(f);
                List<FileNgram> devidedNgrams;
                int i = 0;
                bool eof = false;
                do
                {
                    devidedNgrams = DivideFileBy(f, count, ref eof);
                    string path = directoryPath + Path.GetFileNameWithoutExtension(f.Path) + "_BIN-FILE-FROM-" + (count * i++) + ".dat";
                    CreatePartialBinaryFile(devidedNgrams, path);
                    Console.WriteLine(path);
                } while (!eof);
            }
        }

        private int GetSuitableCountForDivision(NgramFile file)
        {
            int count;
            count = GetDivisionCountByNumber(file.Next().Words.Length);
            file.ReOpen();
            return count;
        }

        private int GetDivisionCountByNumber(int number)
        {
            switch (number)
            {
                case 1:
                    return 20000000;
                case 2:
                    return 10000000;
                case 3:
                    return 5000000;
                case 4:
                    return 2500000;
                default:
                    throw new Exception("Unknown length of ngrams");
            }
        }

        private List<FileNgram> DivideFileBy(NgramFile file, int count, ref bool endOfFile)
        {
            var ret = new List<FileNgram>();
            Ngram ng;
            int i = 0;
            while ((ng = file.Next()) != null && i++ < count)
            {
                foreach (var w in ng.Words)
                {
                    int id = idTrie.Find(StringRoutines.MyDiacriticsRemover(w));
                    if (id == 0)
                    {
                        throw new Exception("Word '" + w + "' is not in idTrie!!!");
                    }
                    ret.Add(new FileNgram(ng.ToString(), ng.Frequency, id));
                }
            }
            endOfFile = ng == null;
            Console.WriteLine("part of file loaded...");
            return ret;
        }

        internal void CreatePartialBinaryFile(List<FileNgram> ngrams, string path)
        {
            SortByIdAsc(ngrams);
            Console.WriteLine("Sorted...");

            using (var fileWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                for (int id = minId; id <= maxId; id++)
                {
                    var result = FindFileNgramsBinarySearch(ngrams, id);
                    SortByFrequencyDesc(result);

                    fileWriter.Write(result.Count);
                    foreach (var ng in result)
                    {
                        fileWriter.Write(ng.Value);
                    }
                    if (id % 1000000 == 0) { Console.WriteLine("id = " + id); }
                }
            }
        }

        private List<FileNgram> GetFileNgrams(NgramFile file, int from, int to)
        {
            var ngrams = new List<FileNgram>();
            Ngram ng;
            int i = 1;

            while (i < from)
            {
                if ((ng = file.Next()) == null)
                {
                    return ngrams;
                }
                i++;
            }

            while (i <= to)
            {
                if ((ng = file.Next()) != null)
                {
                    foreach (var w in ng.Words)
                    {
                        int id = idTrie.Find(StringRoutines.MyDiacriticsRemover(w));
                        ngrams.Add(new FileNgram(ng.ToString(), ng.Frequency, id));
                    }
                    i++;
                }
                else
                {
                    break;
                }
            }

            return ngrams;
        }

        // Creating files from DB

        internal static void CreateBinaryFileFromDBWordsAndUniGramsEntities(string positionTriePath, string fileUniGramsPath)
        {
            using (var db = new DiacriticsDBEntities())
            {
                db.Database.Connection.Open();

                List<FileNgram> ngrams = GetAllFileNgrams(db);
                Console.WriteLine("rows created...");

                var sqlSelectWord = new SqlCommand("SELECT Id, Value FROM dbo.Words ORDER BY Id ASC",
                    db.Database.Connection as SqlConnection);
                sqlSelectWord.CommandType = CommandType.Text;

                using (SqlDataReader wordReader = sqlSelectWord.ExecuteReader())
                using (var trieWriter = new StreamWriter(positionTriePath))
                using (BinaryWriter fileWriter = new BinaryWriter(File.Open(fileUniGramsPath, FileMode.Create)))
                {
                    Console.WriteLine("started...");
                    while (wordReader.Read())
                    {
                        int id = wordReader.GetInt32(0);
                        string word = wordReader.GetString(1);

                        trieWriter.WriteLine(word + " " + fileWriter.BaseStream.Position);

                        var foundNgrams = FindFileNgramsBinarySearch(ngrams, id);
                        SortByFrequencyDesc(foundNgrams);

                        fileWriter.Write(foundNgrams.Count);
                        foreach (var ng in foundNgrams)
                        {
                            fileWriter.Write(ng.Value);
                        }
                        if (id % 100000 == 0) { Console.WriteLine(id); }
                    }
                }
                db.Database.Connection.Close();
            }
        }

        private static List<FileNgram> GetAllFileNgrams(DiacriticsDBEntities db)
        {
            var sqlSelectUniGrams = new SqlCommand("SELECT Word1, Frequency, WordId FROM dbo.UniGramEntities",
                db.Database.Connection as SqlConnection);
            sqlSelectUniGrams.CommandType = CommandType.Text;

            var ret = new List<FileNgram>();

            using (SqlDataReader unigramsReader = sqlSelectUniGrams.ExecuteReader())
            {
                while (unigramsReader.Read())
                {
                    ret.Add(new FileNgram(unigramsReader.GetString(0), unigramsReader.GetInt32(1), unigramsReader.GetInt32(2)));
                }
            }

            SortByIdAsc(ret);
            return ret;
        }

        // Common

        private static List<FileNgram> FindFileNgramsBinarySearch(List<FileNgram> ngrams, int id)
        {
            var ret = new List<FileNgram>();

            int bottom = 0;
            int top = ngrams.Count - 1;
            bool found = false;

            while (!found)
            {
                int middle = bottom + (int)Math.Ceiling((double)((top - bottom) / 2));
                int index = middle;

                if (bottom > top)
                {
                    return ret;
                }
                else if (ngrams[index].Id == id)
                {
                    do
                    {
                        ret.Add(ngrams[index]);
                        index++;
                    } while (index < ngrams.Count && ngrams[index].Id == id);

                    index = middle - 1;
                    while (index >= 0 && ngrams[index].Id == id)
                    {
                        ret.Add(ngrams[index]);
                        index--;
                    }
                    found = true;
                }
                else if (ngrams[index].Id < id)
                {
                    bottom = index + 1;
                }
                else
                {
                    top = index - 1;
                }
            }

            return ret;
        }

        private static void SortByFrequencyDesc(List<FileNgram> ngrams)
        {
            ngrams.Sort((x, y) => y.Frequency.CompareTo(x.Frequency));
        }

        private static void SortByIdAsc(List<FileNgram> ngrams)
        {
            ngrams.Sort((x, y) => x.Id.CompareTo(y.Id));
        }

    }
}
