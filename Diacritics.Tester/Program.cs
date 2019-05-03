using System;

namespace Diacritics.Tester
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
            {
                PrintInstructions();
                return;
            }

            string original = args[0];
            string reconstructed = args[1];

            if (args.Length == 3)
            {
                int writeSttististics = int.Parse(args[2]);
                if (writeSttististics == 0 || writeSttististics == 1)
                {
                    DiacriticsTester.FindMistakes(original, reconstructed, writeSttististics == 1);
                }
                else
                {
                    PrintInstructions();
                    return;
                }
            }

            if (args.Length == 4)
            {
                string path = args[2];

                int writeSttististics = int.Parse(args[3]);
                if (writeSttististics == 0 || writeSttististics == 1)
                {
                    DiacriticsTester.FindMistakes(original, reconstructed, path, writeSttististics == 1);
                }
                else
                {
                    PrintInstructions();
                    return;
                }
            }

        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Instructions:");

            Console.WriteLine("Arguments: \n" +
                "\t0 -> OriginalTextPath \n" +
                "\t1 -> ReconstructedTextPath \n" +
                "\t2 -> WriteStatistics (0 = false, 1 = true)");

            Console.WriteLine("OR");

            Console.WriteLine(
                "\t0 -> OriginalText \n" +
                "\t1 -> ReconstructedText \n" +
                "\t2 -> Path (path where to write mistakes and statistics) \n" +
                "\t3 -> WriteStatistics (0 = false, 1 = true)");
        }

    }
}
