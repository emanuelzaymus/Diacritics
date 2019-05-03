using System.IO;

namespace Diacritics.Tester
{
    class TextFile
    {

        internal static string FileName(string path) => path.Substring(0, path.LastIndexOf('.'));

        internal static string FileExtension(string path) => Path.GetExtension(path);

    }
}
