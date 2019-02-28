using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DiacriticsProject1.Reconstructors.DBDR
{
    class DBCreator
    {
        internal static void Load(List<NgramFile> files)
        {
            foreach (var f in files)
            {
                Load(f);
                Console.WriteLine($"{f.FileName} loaded into DB");
            }
        }

        internal static void Load(NgramFile file)
        {
            using (var db = new DiacriticsDBEntities())
            {
                switch (file.Next().Words.Length)
                {
                    case 1:
                        LoadUniGramsSqlCmd(file, db);
                        break;
                    default:
                        throw new Exception("Unknown length of ngams!");
                }
                db.SaveChanges();
            }
        }

        private static void LoadUniGramsSqlCmd(NgramFile file, DiacriticsDBEntities db)
        {
            var sqlSelect = new SqlCommand("SELECT * FROM dbo.Words WHERE Value = @value", db.Database.Connection as SqlConnection);
            sqlSelect.CommandType = CommandType.Text;
            sqlSelect.Parameters.Add("value", SqlDbType.NVarChar);

            var sqlInsertWord = new SqlCommand("INSERT INTO dbo.Words (Value) VALUES (@value)", db.Database.Connection as SqlConnection);
            sqlInsertWord.CommandType = CommandType.Text;
            sqlInsertWord.Parameters.Add("value", SqlDbType.NVarChar);

            var sqlInsertUniGram = new SqlCommand("INSERT INTO dbo.UniGramEntities (Word1, WordId, Frequency) VALUES (@word1, @wordId, @frequency)",
                db.Database.Connection as SqlConnection);
            sqlInsertUniGram.CommandType = CommandType.Text;
            sqlInsertUniGram.Parameters.Add("word1", SqlDbType.NVarChar);
            sqlInsertUniGram.Parameters.Add("wordId", SqlDbType.Int);
            sqlInsertUniGram.Parameters.Add("frequency", SqlDbType.Int);

            db.Database.Connection.Open();

            file.ReOpen();
            Ngram ngram;
            int i = 0;
            while ((ngram = file.Next()) != null)
            {
                foreach (var w in ngram.Words)
                {
                    string nonDiacriticsW = StringRoutines.MyDiacriticsRemover(w);
                    int id = -1;
                    bool wasIserted;
                    do
                    {
                        wasIserted = false;
                        sqlSelect.Parameters["value"].Value = nonDiacriticsW;
                        SqlDataReader reader = sqlSelect.ExecuteReader();

                        if (reader.Read())
                        {
                            id = (int)reader[0];
                        }
                        else
                        {
                            sqlInsertWord.Parameters["value"].Value = nonDiacriticsW;
                            sqlInsertWord.ExecuteNonQuery();
                            wasIserted = true;
                        }
                        reader.Close();
                    } while (wasIserted);

                    sqlInsertUniGram.Parameters["word1"].Value = w;
                    sqlInsertUniGram.Parameters["wordId"].Value = id;
                    sqlInsertUniGram.Parameters["frequency"].Value = ngram.Frequency;
                    sqlInsertUniGram.ExecuteNonQuery();
                }
                if (++i % 10000 == 0) Console.WriteLine(i);
            }
            db.Database.Connection.Close();

            sqlSelect.Dispose();
            sqlInsertWord.Dispose();
            sqlInsertUniGram.Dispose();
        }

        private static void LoadUniGrams(NgramFile file, DiacriticsDBEntities db)
        {
            file.ReOpen();
            Ngram ngram;
            Word word;
            int i = 0;
            while ((ngram = file.Next()) != null)
            {
                foreach (var w in ngram.Words)
                {
                    string nonDiacriticsW = StringRoutines.MyDiacriticsRemover(w);
                    word = db.Words.Where(a => a.Value == nonDiacriticsW).SingleOrDefault();
                    if (word == null)
                    {
                        word = new Word()
                        {
                            Value = nonDiacriticsW
                        };
                        db.Words.Add(word);
                    }
                    db.UniGramEntities.Add(new UniGramEntity()
                    {
                        Frequency = ngram.Frequency,
                        Word = word,
                        WordId = word.Id,
                        Word1 = w
                    });
                }
                if (++i % 100 == 0)
                {
                    db.SaveChanges();
                    Console.WriteLine(i);
                }
            }
        }

    }
}
