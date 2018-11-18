using Diacritisc_project1;
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

        public static string charsPattern = $"[{latinChars}{nonLatinChars}]";
        private string digitsPattern = $"[{digits}]";

        internal string CompleteProcessing(NgramFile file, int rmvWordsFromFreq = 0, int rmvBadWordsFromFreq = int.MaxValue)
        {
            if (rmvWordsFromFreq != 0)
            {
                file = new NgramFile(RemoveWordsFromFreqDown(file, rmvWordsFromFreq), file.GetFileType());
            }
            file = new NgramFile(Clean(file), file.GetFileType());
            return RemoveBadWords(file, rmvBadWordsFromFreq);
        }
        internal string RemoveWordsFromFreqDown(NgramFile file, int fromFrequency)
        {
            string name = file.FileName;
            string extension = file.FileExtension;

            using (var fromDown_sw = new StreamWriter($"{name}_FROM-{fromFrequency}-DOWN{extension}"))
            using (var to_sw = new StreamWriter($"{name}_TO-{fromFrequency}{extension}"))
            {
                Ngram ngram;
                while ((ngram = file.Next()) != null)
                {
                    if (ngram.Frequency <= fromFrequency)
                    {
                        fromDown_sw.WriteLine(ngram.Line);
                    }
                    else
                    {
                        to_sw.WriteLine(ngram.Line);
                    }
                }
            }
            return $"{name}_TO-{fromFrequency}{extension}";
        }

        internal string Clean(NgramFile file)
        {
            string name = file.FileName;
            string extension = file.FileExtension;
            string word;

            using (var cleaned_sw = new StreamWriter($"{name}_CLEANED{extension}"))
            using (var chrs_nums_sw = new StreamWriter($"{name}_TRASH-CHRS+NUMS{extension}"))
            using (var nums_sw = new StreamWriter($"{name}_TRASH-NUMS{extension}"))
            using (var trash_sw = new StreamWriter($"{name}_TRASH{extension}"))
            {
                Ngram ngram;
                while ((ngram = file.Next()) != null)
                {
                    word = join(ngram.Words, false);

                    if (Regex.IsMatch(word, charsPattern) && Regex.IsMatch(word, digitsPattern))
                    {
                        chrs_nums_sw.WriteLine(ngram.Line);
                    }
                    else if (Regex.IsMatch(word, digitsPattern))
                    {
                        nums_sw.WriteLine(ngram.Line);
                    }
                    else if (Regex.IsMatch(word, $@"^{charsPattern}+$"))
                    {
                        cleaned_sw.WriteLine(ngram.Line);
                    }
                    else
                    {
                        trash_sw.WriteLine(ngram.Line);
                    }
                }
            }
            return $"{name}_CLEANED{extension}";
        }

        internal string RemoveBadWords(NgramFile file, int fromFrequency = int.MaxValue)
        {
            string name = file.FileName;
            string extension = file.FileExtension;

            using (var goodWords_sw = new StreamWriter($"{name}_GOOD-WORDS{extension}"))
            using (var badWords_sw = new StreamWriter($"{name}_BAD-WORDS{extension}"))
            {
                Ngram ngram;
                while ((ngram = file.Next()) != null)
                {
                    if (ngram.Frequency > fromFrequency || isGoodWord(RemoveDiacritics(join(ngram.Words, true))))
                    {
                        goodWords_sw.WriteLine(ngram.Line);
                    }
                    else
                    {
                        badWords_sw.WriteLine(ngram.Line);
                    }
                }
            }
            return $"{name}_GOOD-WORDS{extension}";
        }

        private string join(string[] words, bool withWhiteSpaces)
        {
            if (withWhiteSpaces)
            {
                return String.Join(" ", words);
            }
            else
            {
                return String.Join("", words);
            }
        }

        public static string RemoveDiacritics(string word)
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