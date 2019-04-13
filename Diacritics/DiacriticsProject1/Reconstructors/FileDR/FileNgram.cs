namespace DiacriticsProject1.Reconstructors.FileDR
{
    class FileNgram
    {
        public string Value { get; }
        public int Frequency { get; }
        public int Id { get; }

        public FileNgram(string value, int frequency, int id)
        {
            Value = value;
            Frequency = frequency;
            Id = id;
        }

    }
}
