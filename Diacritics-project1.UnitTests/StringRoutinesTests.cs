using NUnit.Framework;
using DiacriticsProject1.Common;

namespace Tests
{
    [TestFixture]
    public class StringRoutinesTests
    {
        [Test]
        public void RemoveDiacritics_WordWithDiacritics_ReturnsWithoutDiacritics()
        {
            string input = "áäčďéíĺľňóôŕšťúýžěřůäöüẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";
            string expected = "aacdeillnoorstuyzeruaouẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";

            string result = StringRoutines.RemoveDiacritics(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void MyDiacriticsRemover_WordWithDiacritics_ReturnsWithoutDiacritics()
        {
            string input = "áäčďéíĺľňóôŕšťúýžěřůäöüẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";
            string expected = "aacdeillnoorstuyzeruaouẞß abcdefghijklmnopqrstuvwxyz ,./;'[]{} 1234567890 ~`!@#$%^&*()+_-";

            string result = StringRoutines.MyDiacriticsRemover(input);

            Assert.AreEqual(expected, result);
        }

    }
}