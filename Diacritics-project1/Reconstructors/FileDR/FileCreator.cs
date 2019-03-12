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
        class DBRow
        {
            public string Word1 { get; }
            public int WordId { get; }
            public int Frequency { get; }

            public DBRow(string word1, int wordId, int frequency)
            {
                Word1 = word1;
                WordId = wordId;
                Frequency = frequency;
            }
        }

        public static void CreateBinaryFile()
        {
            using (var db = new DiacriticsDBEntities())
            {
                db.Database.Connection.Open();

                List<DBRow> rows = GetAllDBRows(db);
                Console.WriteLine("rows created...");
                //DataTable dt = sqlSelectUniGrams.ExecuteReader().GetSchemaTable();

                var sqlSelectWord = new SqlCommand("SELECT Id, Value FROM dbo.Words ORDER BY Id ASC",
                    db.Database.Connection as SqlConnection);
                sqlSelectWord.CommandType = CommandType.Text;

                using (SqlDataReader wordReader = sqlSelectWord.ExecuteReader())
                using (var trieWriter = new StreamWriter("D:/binFile/fileTrie.txt"))
                using (BinaryWriter fileWriter = new BinaryWriter(File.Open("D:/binFile/data.dat", FileMode.Create)))
                {
                    Console.WriteLine("started...");
                    while (wordReader.Read())
                    {
                        int id = wordReader.GetInt32(0);
                        string word = wordReader.GetString(1);

                        trieWriter.WriteLine(word + " " + fileWriter.BaseStream.Position);

                        var bdRows = GetWordsBinarySearch(rows, id);
                        fileWriter.Write(bdRows.Count);
                        foreach (var ng in bdRows)
                        {
                            fileWriter.Write(ng.Word1);
                        }
                        if (id % 100000 == 0) { Console.WriteLine(id); }
                    }
                }
                db.Database.Connection.Close();
            }
        }

        private static List<DBRow> GetWordsBinarySearch(List<DBRow> rows, int wordId)
        {
            var ret = new List<DBRow>();

            int bottom = 0;
            int top = rows.Count - 1;
            bool found = false;

            while (!found)
            {
                int middle = bottom + (int)Math.Ceiling((double)((top - bottom) / 2));
                int index = middle;

                if (rows[index].WordId == wordId)
                {
                    do
                    {
                        ret.Add(rows[index]);
                        index++;
                    } while (index < rows.Count && rows[index].WordId == wordId);

                    index = middle - 1;
                    while (index >= 0 && rows[index].WordId == wordId)
                    {
                        ret.Add(rows[index]);
                        index--;
                    }
                    found = true;
                }
                else if (bottom == top) 
                {
                    throw new Exception("Id not found!");
                }
                else if (rows[index].WordId < wordId)
                {
                    bottom = index + 1;
                }
                else
                {
                    top = index - 1;
                }
            }

            ret.Sort((x, y) => y.Frequency.CompareTo(x.Frequency));
            return ret;
        }

        private static List<DBRow> GetAllDBRows(DiacriticsDBEntities db)
        {
            var sqlSelectUniGrams = new SqlCommand("SELECT Word1, WordId, Frequency FROM dbo.UniGramEntities",
                db.Database.Connection as SqlConnection);
            sqlSelectUniGrams.CommandType = CommandType.Text;

            var ret = new List<DBRow>();

            using (SqlDataReader unigramsReader = sqlSelectUniGrams.ExecuteReader())
            {
                while (unigramsReader.Read())
                {
                    ret.Add(new DBRow(unigramsReader.GetString(0), unigramsReader.GetInt32(1), unigramsReader.GetInt32(2)));
                }
            }

            ret.Sort((x, y) => x.WordId.CompareTo(y.WordId));
            return ret;
        }

        internal static void Test(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            using (BinaryWriter fileWriter = new BinaryWriter(File.Open("D:/binFile/data.dat", FileMode.Create)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    fileWriter.Write(line);
                }
            }
        }

    }
}
