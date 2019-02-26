using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiacriticsProject1.Reconstructors.DBDR
{
    class DBDR : DRBase
    {
        public DiacriticsDBEntities db { get; set; }

        protected override bool SetDiacritics(ref string word, string[] nthBefore, string[] nthAfter)
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
    }
}
