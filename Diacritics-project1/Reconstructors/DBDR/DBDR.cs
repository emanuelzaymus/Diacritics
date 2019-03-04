using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PBCD.Algorithms.DataStructure;

namespace DiacriticsProject1.Reconstructors.DBDR
{
    class DBDR : DRBase, IDisposable
    {
        private DiacriticsDBEntities db;

        private SqlCommand sqlSelectUniGrams;

        private Trie<char, int> wordTrie;

        public DBDR()
        {
            db = new DiacriticsDBEntities();

            wordTrie = DBTrieCreator.CreateDBTrie(db);

            db.Database.Connection.Open();

            sqlSelectUniGrams = new SqlCommand("SELECT Word1 FROM dbo.UniGramEntities WHERE WordId = @id ORDER BY Frequency DESC",
                db.Database.Connection as SqlConnection);
            sqlSelectUniGrams.CommandType = CommandType.Text;
            sqlSelectUniGrams.Parameters.Add("id", SqlDbType.Int);
        }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
        {
            return SetDiacriticsSqlCmd(ref word, nthBefore, nthAfter);
        }

        private bool SetDiacriticsSqlCmd(ref string word, string[] nthBefore, string[] nthAfter)
        {
            int id = wordTrie.Find(word);
            if (id == 0)
            {
                return false;
            }
            sqlSelectUniGrams.Parameters["id"].Value = id;

            using (SqlDataReader reader = sqlSelectUniGrams.ExecuteReader())
            {
                string result = null;
                while (reader.Read())
                {
                    string[] ngrmWords = { (string)reader[0] };
                    if (base.MatchesUp(word, ngrmWords, nthBefore, nthAfter, ref result))
                    {
                        word = result;
                        return true;
                    }
                }
            }
            throw new Exception("No match in ngrams!");
        }

        private bool SetDiacriticsEF(ref string word, string[] nthBefore, string[] nthAfter)
        {
            string w = word;
            Word DBWord = db.Words.Where(a => a.Value == w).SingleOrDefault();
            if (DBWord == null)
            {
                return false;
            }
            var ngrams = db.UniGramEntities.Where(a => a.WordId == DBWord.Id).OrderByDescending(a => a.Frequency).ToList();

            string result = null;
            foreach (var ng in ngrams)
            {
                string[] ngrmWords = { ng.Word1 };
                if (base.MatchesUp(word, ngrmWords, nthBefore, nthAfter, ref result))
                {
                    word = result;
                    return true;
                }
            }
            throw new Exception("No match in ngrams!");
        }

        public void Dispose()
        {
            db.Database.Connection.Close();
            db.Dispose();
        }

    }
}
