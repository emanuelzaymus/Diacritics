using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diacritisc_project1
{
    class FileHandler
    {

        public static string ReadUTF8(string path)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static void WriteUTF8(string path, string text)
        {
            File.WriteAllText(path, text);
        }

    }
}
