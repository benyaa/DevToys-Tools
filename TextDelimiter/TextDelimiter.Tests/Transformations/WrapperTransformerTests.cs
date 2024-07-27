
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
            List<ReadOnlyMemory<char>> input = new List<ReadOnlyMemory<char>>
                {
                    "Hello,".AsMemory(),
                    "world!".AsMemory()
                };
            List<string> expectedOutput = new List<string> { "<Hello,>", "<world!>" };

            IEnumerable<ReadOnlyMemory<char>> result = transformer.Transform(input);

            Assert.That(result.Select(r => r.ToString()), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Transform_ShouldNotWrapEmptyText()
        {
            WrapperTransformer transformer = new WrapperTransformer("[", "]");
            List<ReadOnlyMemory<char>> input = new List<ReadOnlyMemory<char>>();
            List<string> expectedOutput = new List<string>();

            IEnumerable<ReadOnlyMemory<char>> result = transformer.Transform(input);

            Assert.That(result.Select(r => r.ToString()), Is.EqualTo(expectedOutput));
        }
    }
}