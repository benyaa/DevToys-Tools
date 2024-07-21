using NUnit.Framework;
using System.Collections.Generic;
using TextDelimiter.Transformations;

namespace TextDelimiter.Tests
{
    [TestFixture]
    public class AttackTheClonesTransformerTests
    {
        [Test]
        public void RemoveDuplicateRecords_ShouldRemoveDuplicates()
        {
            AttackTheClonesTransformer transformer = new AttackTheClonesTransformer();
            List<string> inputRecords =
            [
                "record1",
                "record2",
                "record1",
                "record3",
                "record2"
            ];

            IEnumerable<string> result = transformer.Transform(inputRecords);

            Assert.That(result.Count(), Is.EqualTo(3), "The count of the result should be 3.");
            Assert.That(result, Contains.Item("record1"), "The result should contain 'record1'.");
            Assert.That(result, Contains.Item("record2"), "The result should contain 'record2'.");
            Assert.That(result, Contains.Item("record3"), "The result should contain 'record3'.");
        }
    }
}