using System.IO;

namespace DiacriticsProject1.Common.Files
{
    class TextFile
    {

        internal static string FileName(string path)
        {
            if (path.LastIndexOf('.') < 0)
            {
                return path;
            }
            return path.Substring(0, path.LastIndexOf('.'));
        }

        internal static string FileExtension(string path) =>
            Path.GetExtension(path);  //path.Substring(path.LastIndexOf('.'));

    }
}
