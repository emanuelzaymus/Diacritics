using System;
using System.IO;
using System.Text.RegularExpressions;


namespace DiacriticsProject1
{
    /// <summary>
    /// 
    /// </summary>
    internal class FileCleaner
    {
        private static string latinChars = "a-z";
        private static string nonLatinChars = "áäčďéíĺľňóôŕšťúýžěřůöüẞß";
        private static string digits = "0-9";

        public static Regex rgxChars = new Regex($"[{latinChars}{nonLatinChars}]");

        private Regex rgxNonChars;
        private Regex rgxDigits;

        public FileCleaner()
        {
            rgxNonChars = new Regex($@"^[{latinChars}{nonLatinChars}]+$");
            rgxDigits = new Regex($"[{digits}]");
        }

        internal string CompleteProcessing(NgramFile file, int rmvWordsFromFreq = 0, int rmvBadWordsFromFreq = int.MaxValue)
        {
            if (rmvWordsFromFreq > 0)
            {
                file = (file is UniGramFile)
                    ? new UniGramFile(RemoveWordsFromFreqDown(file, rmvWordsFromFreq))
                    : file = new NgramFile(RemoveWordsFromFreqDown(file, rmvWordsFromFreq));
            }
            file = (file is UniGramFile) ? file = new UniGramFile(Clean(file)) : file = new NgramFile(Clean(file));
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
                    //if (ngram.Frequency > fromFrequency || isGoodWord(RemoveDiacritics(String.Join(" ", ngram.Words))))
                    if (ngram.Frequency > fromFrequency || IsGoodWord(StringRoutines.MyDiacriticsRemover(String.Join(" ", ngram.Words))))
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