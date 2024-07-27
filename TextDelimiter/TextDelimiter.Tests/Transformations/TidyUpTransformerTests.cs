using TextDelimiter.Transformations;

namespace TextDelimiter.Tests.transformations
{
    [TestFixture]
    public class TidyUpTransformerTests
    {
        [Test]
        public void Transform_WhenCalled_RemoveRecordsNewLines()
        {
            TidyUpTransformer transformation = new TidyUpTransformer();
            List<ReadOnlyMemory<char>> input = new List<ReadOnlyMemory<char>>
                                                                            {
                                                                                "Hello\n\n".AsMemory(),
                                                                                "Wo\nrld".AsMemory(),
                                                                                "!".AsMemory()
                                                                            };
            List<string> expectedOutput = new List<string> { "Hello", "World", "!" };

            IEnumerable<ReadOnlyMemory<char>> result = transformation.Transform(input);

            Assert.That(result.Select(r => r.ToString()), Is.EqualTo(expectedOutput));
        }
    }
}