using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diacritisc_project1
{
    internal class NgramFile
    {
        private readonly string path;
        private readonly Type type;

        private StreamReader reader;

        public enum Type
        {
            Dictionary,
            Ngrams
        }

        public NgramFile(string path, Type t)
        {
            this.path = path;
            this.type = t;
        }

        internal string FileName => path.Substring(0, path.LastIndexOf('.'));

        internal string FileExtension => path.Substring(path.LastIndexOf('.'));

        internal Type GetFileType() => type;

        internal Ngram Next()
        {
            if (reader == null)
            {
                ReOpen();
            }
            string line = reader.ReadLine();
            return (line != null) ? new Ngram(line, type) : null;
        }

        internal void ReOpen()
        {
            reader = File.OpenText(path);
        }

    }
}
