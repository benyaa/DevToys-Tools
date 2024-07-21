
using TextDelimiter.Transformations;

namespace TextDelimiter.Tests.Transformations
{
    [TestFixture]
    public class WrapperTransformerTests
    {
        [Test]
        public void Transform_ShouldWrapTextWithPrefixAndSuffix()
        {

            WrapperTransformer transformer = new WrapperTransformer("<", ">");
            List<string> input = ["Hello,", "world!"];
            List<string> expectedOutput = ["<Hello,>", "<world!>"];

            IEnumerable<string> result = transformer.Transform(input);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Transform_ShouldNotWrapEmptyText()
        {
            WrapperTransformer transformer = new WrapperTransformer("[", "]");
            List<string> input = [];
            List<string> expectedOutput = [];

            IEnumerable<string> result = transformer.Transform(input);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }
}