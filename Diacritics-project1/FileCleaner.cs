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
        private static string nonLatinChars = "áäčďéíĺľňóôŕšťúýžěřůöüẞß";
        private static string digits = "0-9";

        private string nonDiacriticsCharsPattern = $"[{latinChars}]";
        private string diacriticsCharsPattern = $"[{nonLatinChars}]";

        public static string charsPattern = $"[{latinChars}{nonLatinChars}]";
        private string digitsPattern = $"[{digits}]";

        internal string CompleteProcessing(NgramFile file, int rmvWordsFromFreq = 0, int rmvBadWordsFromFreq = int.MaxValue)
        {
            if (rmvWordsFromFreq != 0)
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
                    //if (ngram.Frequency > fromFrequency || isGoodWord(RemoveDiacritics(String.Join(" ", ngram.Words))))
                    if (ngram.Frequency > fromFrequency || IsGoodWord(MyDiacriticsRemover(String.Join(" ", ngram.Words))))
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

        public static string RemoveDiacritics(string word)  // TODO: StringRoutines
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

        public static string MyDiacriticsRemover(string word)
        {
            var sb = new StringBuilder();
            foreach (var ch in word)
            {
                // dictionary TODO
                switch (ch)
                {
                    case 'á':
                        sb.Append('a');
                        break;
                    case 'ä':
                        sb.Append('a');
                        break;
                    case 'č':
                        sb.Append('c');
                        break;
                    case 'ď':
                        sb.Append('d');
                        break;
                    case 'é':
                        sb.Append('e');
                        break;
                    case 'í':
                        sb.Append('i');
                        break;
                    case 'ĺ':
                        sb.Append('l');
                        break;
                    case 'ľ':
                        sb.Append('l');
                        break;
                    case 'ň':
                        sb.Append('n');
                        break;
                    case 'ó':
                        sb.Append('o');
                        break;
                    case 'ô':
                        sb.Append('o');
                        break;
                    case 'ŕ':
                        sb.Append('r');
                        break;
                    case 'š':
                        sb.Append('s');
                        break;
                    case 'ť':
                        sb.Append('t');
                        break;
                    case 'ú':
                        sb.Append('u');
                        break;
                    case 'ý':
                        sb.Append('y');
                        break;
                    case 'ž':
                        sb.Append('z');
                        break;
                    case 'ě':
                        sb.Append('e');
                        break;
                    case 'ř':
                        sb.Append('r');
                        break;
                    case 'ů':
                        sb.Append('u');
                        break;
                    case 'ö':
                        sb.Append('o');
                        break;
                    case 'ü':
                        sb.Append('u');
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
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