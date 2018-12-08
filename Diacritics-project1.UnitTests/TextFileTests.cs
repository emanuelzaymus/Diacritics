using NUnit.Framework;
using Diacritisc_project1;


namespace Tests
{
    [TestFixture]
    public class TextFileTests
    {
        private string path = "D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS.txt";

        [Test]
        public void FileName_Path_ReturnsFileName()
        {
            var name = "D:/slovniky/prim-8.0-public-all-word_frequency_non_case_sensitive/prim-8.0-public-all-word_frequency_non_case_sensitive_CLEANED_GOOD-WORDS";
            var result = TextFile.FileName(path);

            Assert.AreEqual(result, name);
        }

        [Test]
        public void FileExtension_Path_ReturnsFileExtension()
        {
            var extension = ".txt";
            var result = TextFile.FileExtension(path);

            Assert.AreEqual(result, extension);
        }

    }
}
