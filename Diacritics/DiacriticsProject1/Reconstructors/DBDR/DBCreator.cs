using DiacriticsProject1.Common;
using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.DBDR
{
    class DBCreator
    {
        private Trie<char, int> wordTrie;

        public DBCreator()
        {
            using (var db = new DiacriticsDBEntities())
            {
                wordTrie = DBTrieCreator.CreateDBTrie(db);
            }
        }

        internal void LoadFiles(List<NgramFile> files)
        {
            foreach (var f in files)
            {
                LoadFile(f);
                Console.WriteLine($"{f.FileName} inserted into DB");
            }
        }

        internal void LoadFile(NgramFile file)
        {
            using (var db = new DiacriticsDBEntities())
            {
                switch (file.Next().Words.Length)
                {
                    case 1:
                        InsertUnigramsSqlBulkCopy((UniGramFile)file, db);
                        break;
                    default:
                        throw new Exception("Unknown length of ngams!");
                }
            }
        }

        private void InsertUnigramsSqlBulkCopy(UniGramFile file, DiacriticsDBEntities db)
        {
            var dtUniGrams = new DataTable();
            dtUniGrams.Columns.Add("Word1");
            dtUniGrams.Columns.Add("WordId");
            dtUniGrams.Columns.Add("Id");
            dtUniGrams.Columns.Add("Frequency");

            file.ReOpen();
            Ngram ng;
            int wordId;
            var uniGramId = 0;
            var counter = 0;

            while ((ng = file.Next()) != null)
            {
                string w = ng.ToString();
                string nonDiacriticsW = StringRoutines.MyDiacriticsRemover(w);

                wordId = wordTrie.Find(nonDiacriticsW);
                if (wordId != 0)
                {
                    dtUniGrams.Rows.Add(w, wordId, ++uniGramId, ng.Frequency);
                }
                else
                {
                    throw new Exception("Word '" + nonDiacriticsW + "' is not present in Trie!");
                }
                if (++counter % 10000 == 0) Console.WriteLine(counter + " unigrams prepared for insertion.");
            }
            InsertIntoDb(dtUniGrams, db, "dbo.UniGramEntities");
        }

        private void InsertWordsSqlBulkCopy(UniGramFile file, DiacriticsDBEntities db)
        {
            var dtWords = new DataTable();
            dtWords.Columns.Add("Id");
            dtWords.Columns.Add("Value");

            file.ReOpen();
            Ngram ng;
            int id = 0;
            var counter = 0;

            while ((ng = file.Next()) != null)
            {
                string nonDiacriticsW = StringRoutines.MyDiacriticsRemover(ng.ToString());
                dtWords.Rows.Add(++id, nonDiacriticsW);

                if (++counter % 100000 == 0) Console.WriteLine(counter + " words prepared for insertion.");
            }
            InsertIntoDb(dtWords, db, "dbo.Words");
        }

        private void InsertIntoDb(DataTable dt, DiacriticsDBEntities db, string destinationTableName)
        {
            using (var sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                sqlBulk.BatchSize = 10000;
                sqlBulk.NotifyAfter = 10000;
                sqlBulk.SqlRowsCopied += (sender, eventArgs) => Console.WriteLine("Inserted " + eventArgs.RowsCopied + " records.");

                sqlBulk.DestinationTableName = destinationTableName;
                sqlBulk.WriteToServer(dt);
            }
        }

        private void LoadUniGramsSqlCmd(NgramFile file, DiacriticsDBEntities db)
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

        private void LoadUniGramsEF(NgramFile file, DiacriticsDBEntities db)
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
