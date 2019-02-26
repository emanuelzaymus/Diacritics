using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        LoadUniGramsSqlQuery(file, db);
                        break;
                    default:
                        throw new Exception("Unknown length of ngams!");
                }
                db.SaveChanges();
            }
        }

        private static void LoadUniGramsSqlQuery(NgramFile file, DiacriticsDBEntities db)
        {
            //var sqlSelect = new SqlCommand("SELECT * FROM dbo.Words WHERE Value = @value", db.Database.Connection as SqlConnection);
            //sqlSelect.CommandType = CommandType.Text;
            var sqlInsertWord = new SqlCommand("INSERT INTO dbo.Words (Value) VALUES (@value)", db.Database.Connection as SqlConnection);
            sqlInsertWord.CommandType = CommandType.Text;
            var sqlInsertUniGram = new SqlCommand("INSERT INTO dbo.UniGramEntities (Word1, WordId, Frequency) VALUES (@word1, @wordId, @frequency)", db.Database.Connection as SqlConnection);
            sqlInsertUniGram.CommandType = CommandType.Text;

            db.Database.Connection.Open();

            file.ReOpen();
            Ngram ngram;
            Word word;
            int i = 0;
            while ((ngram = file.Next()) != null)
            {
                foreach (var w in ngram.Words)
                {
                    string nonDiacriticsW = StringRoutines.MyDiacriticsRemover(w);
                    //word = db.Words.Where(a => a.Value == nonDiacriticsW).SingleOrDefault();
                    word = db.Database.SqlQuery<Word>("SELECT * FROM dbo.Words WHERE Value = @p0", nonDiacriticsW).FirstOrDefault();

                    if (word == null)
                    {
                        word = new Word()
                        {
                            Value = nonDiacriticsW
                        };
                        //db.Words.Add(word);
                        sqlInsertWord.Parameters.AddWithValue("value", nonDiacriticsW);
                        sqlInsertWord.ExecuteNonQuery();
                    }
                    //db.UniGramEntities.Add(new UniGramEntity()
                    //{
                    //    Word1 = w,
                    //    WordId = word.Id,
                    //    Frequency = ngram.Frequency,
                    //    Word = word
                    //});
                    sqlInsertUniGram.Parameters.AddWithValue("word1", w);
                    sqlInsertUniGram.Parameters.AddWithValue("wordId", word.Id);
                    sqlInsertUniGram.Parameters.AddWithValue("frequency", ngram.Frequency);
                    sqlInsertUniGram.ExecuteNonQuery();
                }
                if (++i % 100 == 0)
                {
                    db.SaveChanges();
                    Console.WriteLine(i);
                }
            }

            //sqlSelect.Dispose();
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
