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

        protected string charsPattern = $"[{latinChars}{nonLatinChars}]";
        protected string digitsPattern = $"[{digits}]";

        internal string CompleteProcessing(string path, int rmvWordsFromFreq = 0, int rmvBadWordsFromFreq = int.MaxValue)
        {
            if (rmvWordsFromFreq != 0)
            {
                path = RemoveWordsFromFreqDown(path, rmvWordsFromFreq);
            }
            path = Clean(path);
            return RemoveBadWords(path, rmvBadWordsFromFreq);
        }
        internal string RemoveWordsFromFreqDown(string path, int fromFrequency)
        {
            parsePath(path, out string name, out string extension);
            string line;

            using (StreamReader sr = File.OpenText(path))
            using (var fromDown_sw = new StreamWriter($"{name}_FROM-{fromFrequency}-DOWN{extension}"))
            using (var to_sw = new StreamWriter($"{name}_TO-{fromFrequency}{extension}"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (getFrequency(line) <= fromFrequency)
                    {
                        fromDown_sw.WriteLine(line);
                    }
                    else
                    {
                        to_sw.WriteLine(line);
                    }
                }
            }
            return $"{name}_TO-{fromFrequency}{extension}";
        }

        internal string Clean(string path)
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
                    word = getWord(line);

                    if (Regex.IsMatch(word, charsPattern) && Regex.IsMatch(word, digitsPattern))
                    {
                        chrs_nums_sw.WriteLine(line);
                    }
                    else if (Regex.IsMatch(word, digitsPattern))
                    {
                        nums_sw.WriteLine(line);
                    }
                    else if (Regex.IsMatch(word, $@"^{charsPattern}+$"))
                    {
                        cleaned_sw.WriteLine(line);
                    }
                    else
                    {
                        trash_sw.WriteLine(line);
                    }
                }
            }
            return $"{name}_CLEANED{extension}";
        }

        internal string RemoveBadWords(string path, int fromFrequency = int.MaxValue)
        {
            parsePath(path, out string name, out string extension);
            string line;

            using (StreamReader sr = File.OpenText(path))
            using (var goodWords_sw = new StreamWriter($"{name}_GOOD-WORDS{extension}"))
            using (var badWords_sw = new StreamWriter($"{name}_BAD-WORDS{extension}"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (getFrequency(line) > fromFrequency || isGoodWord(removeDiacritics(getWord(line))))
                    {
                        goodWords_sw.WriteLine(line);
                    }
                    else
                    {
                        badWords_sw.WriteLine(line);
                    }
                }
            }
            return $"{name}_GOOD-WORDS{extension}";
        }

        //internal void RemoveWordsWithoutDiacritics(string path)
        //{
        //    parsePath(path, out string name, out string extension);
        //    string line, word;

        //    using (StreamReader sr = File.OpenText(path))
        //    using (var withDiac_sw = new StreamWriter($"{name}_WITH-DIACRITICS{extension}"))
        //    using (var withoutDiac_sw = new StreamWriter($"{name}_WITHOUT-DIACRITICS{extension}"))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            word = getWord(line);

        //            if (Regex.IsMatch(word, diacriticsCharsPattern))
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

        private static string getWord(string line)
        {
            return line.Substring(0, line.IndexOf("\t"));
        }

        private static int getFrequency(string line)
        {
            string frequencyStr = line.Substring(line.IndexOf("\t") + 1);
            return Convert.ToInt32(frequencyStr);
        }

        private string removeDiacritics(string word)
        {
            var normalizedString = word.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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

        private static void parsePath(string path, out string name, out string extension)
        {
            name = path.Substring(0, path.LastIndexOf('.'));
            extension = path.Substring(path.LastIndexOf('.'));
        }

        internal void ReadingSpeedTest(string path)
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