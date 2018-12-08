﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Diacritisc_project1
{
    internal class NgramFile : TextFile
    {
        private readonly string path;

        protected StreamReader reader;

        public NgramFile(string path)
        {
            this.path = path;
        }

        internal string FileName() => FileName(path);

        internal string FileExtension() => FileExtension(path);

        internal virtual Ngram Next()
        {
            if (reader == null)
            {
                ReOpen();
            }
            string line = reader.ReadLine();
            return (line != null) ? new Ngram(line) : null;
        }

        internal void ReOpen()
        {
            reader = File.OpenText(path);
        }

    }
}
