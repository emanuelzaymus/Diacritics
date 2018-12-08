using NUnit.Framework;
using Diacritics_project1;


namespace Tests
{
    [TestFixture]
    public class FileCleanerTests
    {
        [Test]
        public void RemoveDiacritics_WordWithDiacritics_ReturnsWithoutDiacritics()
        {
            string input = "áäčďéíĺľňóôŕšťúýžěřůäöüẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";
            string expected = "aacdeillnoorstuyzeruaouẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";

            string result = FileCleaner.RemoveDiacritics(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void MyDiacriticsRemover_WordWithDiacritics_ReturnsWithoutDiacritics()
        {
            string input = "áäčďéíĺľňóôŕšťúýžěřůäöüẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";
            string expected = "aacdeillnoorstuyzeruaouẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";

            string result = FileCleaner.MyDiacriticsRemover(input);

            Assert.AreEqual(expected, result);
        }

    }
}