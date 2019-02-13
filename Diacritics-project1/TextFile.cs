using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Diacritics_project1
{
    class TextFile
    {

        internal static string FileName(string path) => Path.GetFileName(path); //path.Substring(0, path.LastIndexOf('.'));

        internal static string FileExtension(string path) =>
            Path.GetExtension(path);  //path.Substring(path.LastIndexOf('.'));

    }
}
