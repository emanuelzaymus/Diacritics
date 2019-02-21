using NUnit.Framework;
using DiacriticsProject1.Common.Ngrams;

namespace Tests
{
    [TestFixture]
    public class NgramTests
    {
        [Test]
        public void Words_MultipleWords_ReturnsArrayOfStrings()
        {
            string line = " 2301 a\tja\tsom\tsa";
            string[] words = { "a", "ja", "som", "sa" };
            Ngram ngram = new Ngram(line);

            var result = ngram.Words;

            Assert.AreEqual(result, words);
        }

        [Test]
        public void Frequency_MultipleWords_ReturnsIntFrequency()
        {
            string line = " 2301 a\tja\tsom\tsa";
            int frequency = 2301;
            Ngram ngram = new Ngram(line);

            var result = ngram.Frequency;

            Assert.AreEqual(result, frequency);
        }

    }
}
