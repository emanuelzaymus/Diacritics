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

        public FileCreator()
        {
            using (var db = new DiacriticsDBEntities())
            {
                idTrie = DBTrieCreator.CreateDBTrie(db);
                maxId = db.Words.Max(x => x.Id);
                minId = db.Words.Min(x => x.Id);
            }
        }

        internal void CreateBinaryFilesFromTextFiles(List<NgramFile> files, string directoryPath)
        {
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
                    CreateBinaryFileFromTextFile(devidedNgrams, path);
                    Console.WriteLine(path);
                } while (!eof);
            }
        }

        private int GetSuitableCountForDivision(NgramFile file)
        {
            int count;
            switch (file.Next().Words.Length)
            {
                case 1:
                    count = 20000000;
                    break;
                case 2:
                    count = 10000000;
                    break;
                case 3:
                    count = 5000000;
                    break;
                case 4:
                    count = 2500000;
                    break;
                default:
                    throw new Exception("Unknown length of ngrams");
            }
            file.ReOpen();
            return count;
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

        internal void CreateBinaryFileFromTextFile(List<FileNgram> ngrams, string path)
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
