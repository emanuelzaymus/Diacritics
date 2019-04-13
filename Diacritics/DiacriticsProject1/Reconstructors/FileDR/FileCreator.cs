using System.Collections.Generic;
using System.Linq;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileCreator
    {

        internal static List<string> ReduceNumberOfNgrams(List<string> ngrams, int[] count)
        {
            var ret = new List<string>();

            int c = 0;
            int lastSize = 4;
            foreach (string ng in ngrams)
            {
                int size = ng.Count(x => x == ' ') + 1; // size = what type of n-gram it actually is

                if (lastSize != size)
                {
                    lastSize = size;
                    c = 0;
                }

                if (c < count[lastSize])
                {
                    ret.Add(ng);
                    c++;
                }
            }

            return ret;
        }

    }
}
