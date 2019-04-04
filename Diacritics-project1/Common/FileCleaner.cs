using DiacriticsProject1.Common.Files;
using DiacriticsProject1.Common.Ngrams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiacriticsProject1.Common
{
    internal class FileCleaner
    {
        private static string latinChars = "a-z";
        private static string nonLatinChars = "áäčďéíĺľňóôŕšťúýžěřůöüẞß";
        private static string digits = "0-9";

        public static Regex rgxChars = new Regex($"[{latinChars}{nonLatinChars}]");
        public static Regex rgxNonLatinChars = new Regex($"[{nonLatinChars}]");

        private Regex rgxNonChars;
        private Regex rgxDigits;

        public FileCleaner()
        {
            rgxNonChars = new Regex($@"^[{latinChars}{nonLatinChars}]+$");
            rgxDigits = new Regex($"[{digits}]");
        }

        internal string CompleteProcessing(NgramFile file, int rmvWordsFromFreq = 0, bool clean = true,
            int rmvBadWordsFromFreq = int.MaxValue, int rmvWordsFromLength = int.MaxValue)
        {
            // todo: toto je zbytocne... NgramFile vzdy ostane UniGramFile
            bool isUniGramFile = file is UniGramFile;

            if (rmvWordsFromFreq > 0)
            {
                file = isUniGramFile
                    ? new UniGramFile(RemoveWordsFromFreqDown(file, rmvWordsFromFreq))
                    : file = new NgramFile(RemoveWordsFromFreqDown(file, rmvWordsFromFreq));
            }
            if (clean)
            {
                file = isUniGramFile ? file = new UniGramFile(Clean(file)) : file = new NgramFile(Clean(file));
            }

            if (rmvWordsFromLength == int.MaxValue)
            {
                return RemoveBadWords(file, rmvBadWordsFromFreq);
            }
            else
            {
                file = isUniGramFile ? file = new UniGramFile(RemoveBadWords(file, rmvBadWordsFromFreq))
                    : file = new NgramFile(RemoveBadWords(file, rmvBadWordsFromFreq));
                return RemoveWordsFromLength(file, rmvWordsFromLength);
            }
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
                    word = String.Join("", ngram.Words);

                    if (rgxChars.IsMatch(word) && rgxDigits.IsMatch(word))
                    {
                        chrs_nums_sw.WriteLine(ngram.Line);
                    }
                    else if (rgxDigits.IsMatch(word))
                    {
                        nums_sw.WriteLine(ngram.Line);
                    }
                    else if (rgxNonChars.IsMatch(word))
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

        public string RemoveBadWords(NgramFile file, int fromFrequency)
        {
            string name = file.FileName;
            string extension = file.FileExtension;

            using (var goodWords_sw = new StreamWriter($"{name}_GOOD-WORDS{extension}"))
            using (var badWords_sw = new StreamWriter($"{name}_BAD-WORDS{extension}"))
            {
                Ngram ngram;
                while ((ngram = file.Next()) != null)
                {
                    if (ngram.Frequency > fromFrequency || IsGoodWord(StringRoutines.MyDiacriticsRemover(ngram.ToString())))
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

        internal string RemoveWordsFromLength(NgramFile file, int fromLength)
        {
            string name = file.FileName;
            string extension = file.FileExtension;

            using (var toLength_sw = new StreamWriter($"{name}_TO-LENGTH-{fromLength}{extension}"))
            using (var fromLength_sw = new StreamWriter($"{name}_FROM-LENGTH-{fromLength}{extension}"))
            {
                bool isToLength;
                Ngram ngram;
                while ((ngram = file.Next()) != null)
                {
                    isToLength = true;
                    foreach (var w in ngram.Words)
                    {
                        if (w.Length > fromLength)
                        {
                            isToLength = false;
                            break;
                        }
                    }

                    if (isToLength)
                    {
                        toLength_sw.WriteLine(ngram.Line);
                    }
                    else
                    {
                        fromLength_sw.WriteLine(ngram.Line);
                    }
                }
            }
            return $"{name}_TO-LENGTH-{fromLength}{extension}";
        }

        private bool IsGoodWord(string word)
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

        internal void SortByLineLength(NgramFile file)
        {
            var words = new List<Ngram>();
            Ngram ng;
            while ((ng = file.Next()) != null)
            {
                words.Add(ng);
            }
            var arr = words.ToArray();
            Array.Sort(arr, (x, y) => x.ToString().Length.CompareTo(y.ToString().Length));

            using (var writer = new StreamWriter($"{file.FileName}_SORTED{file.FileExtension}"))
            {
                foreach (var n in arr)
                {
                    writer.WriteLine(n.ToString() + $" {n.Frequency} ({n.ToString().Length})");
                }
            }

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

        public static void RemoveDiacriticsInFile(string path)
        {
            string originalText = File.OpenText(path).ReadToEnd();
            string textWithoutDiacritics = StringRoutines.MyDiacriticsRemover(originalText);
            File.WriteAllText($"{TextFile.FileName(path)}_WITHOUT-DIACRITICS{TextFile.FileExtension(path)}", textWithoutDiacritics);
        }

        public static void CleanFileFromHiddenChars(string path)
        {
            using (var strmWriter = new StreamWriter($"{TextFile.FileName(path)}_CLEANED{TextFile.FileExtension(path)}"))
            using (var binReader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                var rgxLatin = new Regex($"[{latinChars}]");
                var rgxCommon = new Regex("[-+*/=_—–]");
                while (binReader.BaseStream.Position != binReader.BaseStream.Length)
                {
                    byte b = binReader.ReadByte();
                    char c = Convert.ToChar(b);
                    if (rgxLatin.IsMatch(c.ToString().ToLower()) || char.IsDigit(c)
                        || char.IsPunctuation(c) || char.IsSeparator(c)
                        || char.IsWhiteSpace(c) /*|| rgxCommon.IsMatch(c.ToString())*/)
                    {
                        strmWriter.Write(c);
                    }
                    else
                    {
                        Console.WriteLine($"<{c}>");
                    }
                }
            }
        }

    }
}