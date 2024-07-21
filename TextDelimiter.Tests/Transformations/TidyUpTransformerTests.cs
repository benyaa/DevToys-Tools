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
            List<string> input = [ "Hello\n\n", "Wo\nrld", "!" ];
            List<string> expectedOutput = [ "Hello", "World", "!" ];

            IEnumerable<string> result = transformation.Transform(input);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }
}