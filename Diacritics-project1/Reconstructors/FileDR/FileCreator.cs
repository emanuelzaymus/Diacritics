using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileCreator
    {
        public static void CreateFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = File.Create(path))
            {
                AddText(fs, "áäčďéíĺľňóôŕšťúýžěřůöüẞß");
                AddText(fs, "This is some more text,");
                AddText(fs, "\n and this is on a new line");
            }

            //Open the stream and read it back.
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Console.WriteLine(temp.GetString(b));
                }
            }
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }


        public static void CreateBinaryFile(string path)
        {
            string[] arr1 = { "ahoj", "cau", "bye" };
            string[] arr2 = { "jablko", "hruska", "slivka", "hrozno" };
            string[] arr3 = { "stolicka", "stol" };

            var arrs = new List<string[]> { arr1, arr2, arr3 };

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                foreach (var a in arrs)
                {
                    Console.WriteLine("Position: " + writer.BaseStream.Position);
                    Int32 len = a.Length;
                    writer.Write(len);
                    foreach (var item in a)
                    {
                        writer.Write(item);
                    }
                }
            }

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                reader.BaseStream.Position = 49;

                int length = reader.ReadInt32();
                Console.WriteLine(length);

                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine(reader.ReadString());
                }
            }
        }

    }
}
