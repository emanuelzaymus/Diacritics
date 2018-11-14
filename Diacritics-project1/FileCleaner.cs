using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Diacritics_project1
{
    internal class FileCleaner
    {
        private static string latinChars = "a-z";
        private static string nonLatinChars = "áäčďéíĺľňóôŕšťúýžěřů";
        private static string digits = "0-9";

        private string nonDiacriticsCharsPattern = $"[{latinChars}]";
        private string diacriticsCharsPattern = $"[{nonLatinChars}]";

        private string charsPattern = $"[{latinChars}{nonLatinChars}]";
        private string digitsPattern = $"[{digits}]";

        internal void Clean(string path)
        {
            parsePath(path, out string name, out string extension);
            string line, word;

            using (StreamReader sr = File.OpenText(path))
            using (var cleaned_sw = new StreamWriter($"{name}_CLEANED{extension}"))
            using (var chrs_nums_sw = new StreamWriter($"{name}_TRASH-CHRS+NUMS{extension}"))
            using (var nums_sw = new StreamWriter($"{name}_TRASH-NUMS{extension}"))
            using (var trash_sw = new StreamWriter($"{name}_TRASH{extension}"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    word = line.Substring(0, line.IndexOf("\t"));

                    if (Regex.IsMatch(word, charsPattern) && Regex.IsMatch(word, digitsPattern))
                    {
                        chrs_nums_sw.WriteLine(line);
                    }
                    else if (Regex.IsMatch(word, digitsPattern))
                    {
                        nums_sw.WriteLine(line);
                    }
                    else if (Regex.IsMatch(word, charsPattern))
                    {
                        cleaned_sw.WriteLine(line);
                    }
                    else
                    {
                        trash_sw.WriteLine(line);
                    }
                }
            }
        }
        internal void RemoveBadWords(string path)
        {
            parsePath(path, out string name, out string extension);
            string line, word;

            using (StreamReader sr = File.OpenText(path))
            using (var goodWords_sw = new StreamWriter($"{name}_GOOD-WORDS{extension}"))
            using (var badWords_sw = new StreamWriter($"{name}_BAD-WORDS{extension}"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    word = line.Substring(0, line.IndexOf("\t"));
                    //word = removeDiacritics(word);

                    if (isGoodWord(word))
                    {
                        goodWords_sw.WriteLine(line);
                    }
                    else
                    {
                        badWords_sw.WriteLine(line);
                    }
                }
            }
        }

        private string removeDiacritics(string word)
        {
            //var normalizedString = word.Normalize(NormalizationForm.FormD);
            //var stringBuilder = new StringBuilder();

            //foreach (var c in normalizedString)
            //{
            //    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            //    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            //    {
            //        stringBuilder.Append(c);
            //    }
            //}

            //return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            throw new NotImplementedException();
        }

        private bool isGoodWord(string word)
        {
            char lastChar = ' ';
            bool wasTwoTimes = false;

            foreach (char ch in word)
            {
                if (ch == lastChar)
                {
                    if (wasTwoTimes)
                    {
                        return false;
                    }
                    wasTwoTimes = true;
                }
                else
                {
                    wasTwoTimes = false;
                }
                lastChar = ch;
            }
            return true;
        }

        internal void RemoveWordsWithFreq(string path, int frequency)
        {
            parsePath(path, out string name, out string extension);
            string line, word;

            using (StreamReader sr = File.OpenText(path))
            using (var with_sw = new StreamWriter($"{name}_WITH-{frequency}{extension}"))
            using (var without_sw = new StreamWriter($"{name}_WITHOUT-{frequency}{extension}"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    word = line.Substring(line.IndexOf("\t") + 1);

                    if (Convert.ToInt32(word) == frequency)
                    {
                        with_sw.WriteLine(line);
                    }
                    else
                    {
                        without_sw.WriteLine(line);
                    }
                }
            }
        }

        //internal void RemoveWordsWithoutDiacritics(string path)
        //{
        //    parsePath(path, out string name, out string extension);
        //    string line, subStr;

        //    using (StreamReader sr = File.OpenText(path))
        //    using (var withDiac_sw = new StreamWriter($"{name}_WITH-DIACRITICS{extension}"))
        //    using (var withoutDiac_sw = new StreamWriter($"{name}_WITHOUT-DIACRITICS{extension}"))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            subStr = line.Substring(0, line.IndexOf("\t"));

        //            if (Regex.IsMatch(subStr, diacriticsCharsPattern))
        //            {
        //                withDiac_sw.WriteLine(line);
        //            }
        //            else
        //            {
        //                withoutDiac_sw.WriteLine(line);
        //            }
        //        }
        //    }
        //}

        private static void parsePath(string path, out string name, out string extension)
        {
            name = path.Substring(0, path.LastIndexOf('.'));
            extension = path.Substring(path.LastIndexOf('.'));
        }

        public void ReadingSpeedTest(string path)
        {
            DateTime start, end;
            int counter = 0;

            Console.WriteLine("File.ReadLines:");
            start = DateTime.Now;
            foreach (string line in File.ReadLines(path))
            {
                counter++;
            }
            end = DateTime.Now;
            Console.WriteLine($"Count = {counter}");
            Console.WriteLine($"Time: {end - start}\n");

            counter = 0;

            Console.WriteLine("File.OpenText:");
            string s = String.Empty;
            start = DateTime.Now;
            using (StreamReader sr = File.OpenText(path))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    counter++;
                }
            }
            end = DateTime.Now;
            Console.WriteLine($"Count = {counter}");
            Console.WriteLine($"Time: {end - start}\n");

            counter = 0;

            Console.WriteLine("FileStream -> BufferedStream -> StreamReader:");
            string l;
            start = DateTime.Now;
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                while ((l = sr.ReadLine()) != null)
                {
                    counter++;
                }
            }
            end = DateTime.Now;
            Console.WriteLine($"Count = {counter}");
            Console.WriteLine($"Time: {end - start}\n");

            try
            {
                Console.WriteLine("File.ReadAllLines:");
                start = DateTime.Now;
                string[] lines = File.ReadAllLines(path);
                end = DateTime.Now;
                Console.WriteLine($"Count = {lines.Length}");
                Console.WriteLine($"Time: {end - start}\n");
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }
}