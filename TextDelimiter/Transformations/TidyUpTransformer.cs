namespace TextDelimiter.Transformations
{
    public class TidyUpTransformer : ITextTransformer
    {
        public IEnumerable<string> Transform(IEnumerable<string> input) => input.Select(p => p.Replace(Environment.NewLine, ""));

        public int Order => 100;
    }
}