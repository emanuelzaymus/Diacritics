﻿namespace DiacriticsProject1.Reconstructors
{
    interface IDiacriticsReconstructor
    {
        void Reconstrut(string sourcePath, string destinationPath);

        string Reconstruct(string text);

        string GetStatistic();

        void EraseStatistic();

    }
}
