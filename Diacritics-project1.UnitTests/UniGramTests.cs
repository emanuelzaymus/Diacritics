using NUnit.Framework;
using DiacriticsProject1.Common.Ngrams;

namespace Tests
{
    [TestFixture]
    public class UniGramTests
    {
        [Test]
        public void Words_OneWordLine_ReturnsArrayWithOneString()
        {
            string line = "a\t33198366";
            string[] words = { "a" };
            var uniGram = new UniGram(line);

            var result = uniGram.Words;

            Assert.AreEqual(result, words);
        }

        [Test]
        public void Frequency_OneWordLine_ReturnsIntFrequency()
        {
            string line = "a\t33198366";
            int frequency = 33198366;
            var uniGram = new UniGram(line);

            var result = uniGram.Frequency;

            Assert.AreEqual(result, frequency);
        }

        [Test]
        public void ToString_OneWordLine_ReturnsSimpleWord()
        {
            string line = "a\t33198366";
            string expectedToString = "a";
            var ngram = new UniGram(line);

            var result = ngram.ToString();

            Assert.AreEqual(result, expectedToString);
        }

    }
}
