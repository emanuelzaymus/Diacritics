using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileCreator
    {
        public static void CreateBinaryFile()
        {
            using (var db = new DiacriticsDBEntities())
            {
                db.Database.Connection.Open();

                var sqlSelectUniGrams = new SqlCommand("SELECT Word1 FROM dbo.UniGramEntities WHERE WordId = @id ORDER BY Frequency DESC",
                db.Database.Connection as SqlConnection);
                sqlSelectUniGrams.CommandType = CommandType.Text;
                sqlSelectUniGrams.Parameters.Add("id", SqlDbType.Int);

                var sqlSelectWord = new SqlCommand("SELECT Value FROM dbo.Words WHERE Id = @id",
                db.Database.Connection as SqlConnection);
                sqlSelectWord.CommandType = CommandType.Text;
                sqlSelectWord.Parameters.Add("id", SqlDbType.Int);

                int from = db.Words.Min(w => w.Id);
                int to = db.Words.Max(w => w.Id);

                using (BinaryWriter fileWriter = new BinaryWriter(File.Open("D:/binFile/data.dat", FileMode.Create)))
                using (var trieWriter = new StreamWriter("D:/binFile/fileTrie.txt"))
                {
                    for (int id = from; id < to + 1; id++)
                    {
                        sqlSelectUniGrams.Parameters["id"].Value = id;
                        sqlSelectWord.Parameters["id"].Value = id;

                        using (SqlDataReader unigramsReader = sqlSelectUniGrams.ExecuteReader())
                        using (SqlDataReader wordReader = sqlSelectWord.ExecuteReader())
                        {
                            var ngrms = new List<string>();
                            while (unigramsReader.Read())
                            {
                                ngrms.Add((string)unigramsReader[0]);
                            }
                            wordReader.Read();
                            trieWriter.WriteLine(wordReader.GetString(0) + " " + fileWriter.BaseStream.Position);

                            fileWriter.Write(ngrms.Count);

                            foreach (var ng in ngrms)
                            {
                                fileWriter.Write(ng);
                            }
                        }
                        if (id % 100000 == 0) { Console.WriteLine(id); }
                    }
                }
                db.Database.Connection.Close();
            }
        }

    }
}
