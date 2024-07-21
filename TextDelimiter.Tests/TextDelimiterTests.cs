using NUnit.Framework;

[TestFixture]
public class TextDelimiterTests
{
    [Test]
    public void Transform_ShouldReturnEmptyString_WhenInputIsEmpty()
    {
        TextDelimiter.TextDelimiterTransformer transformer = new TextDelimiter.TextDelimiterTransformer(ExplodeMode.NewLines, "", []);

        string result = transformer.DelimitText("");

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void Transform_ShouldReturnOriginalString_WhenNoDelimitersAreFound()
    {
        TextDelimiter.TextDelimiterTransformer transformer = new TextDelimiter.TextDelimiterTransformer(ExplodeMode.NewLines, "", []);
        string input = "This is a test";

        string result = transformer.DelimitText(input);

        Assert.That(result, Is.EqualTo(input));
    }

    [Test]
    public void Transform_ShouldReturnTransformedString_WhenDelimitersAreFound()
    {
        TextDelimiter.TextDelimiterTransformer transformer = new TextDelimiter.TextDelimiterTransformer(ExplodeMode.Commas, " ", []);
        string input = "Hello,World";
        string expectedOutput = "Hello World";

        string result = transformer.DelimitText(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}