using PBCD.Algorithms.DataStructure;
using System;
using System.Data.SqlClient;

namespace DiacriticsProject1.Reconstructors.DBDR
{
    class DBTrieCreator
    {
        public static Trie<char, int> CreateDBTrie(DiacriticsDBEntities db)
        {
            var t = new Trie<char, int>();

            using (SqlCommand sqlSelect = new SqlCommand("SELECT * FROM dbo.Words", db.Database.Connection as SqlConnection))
            {
                db.Database.Connection.Open();
                Console.WriteLine("Creating word trie...");
                using (SqlDataReader reader = sqlSelect.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        t.Add((string)reader[1], (int)reader[0]);
                    }
                }
                Console.WriteLine("Word trie created.");
                db.Database.Connection.Close();
            }

            return t;
        }

    }
}
