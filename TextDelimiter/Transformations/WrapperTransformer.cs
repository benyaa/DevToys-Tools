namespace TextDelimiter.Transformations
{
    public class WrapperTransformer : ITextTransformer
    {
        public readonly string OpenTag;
        public readonly string CloseTag;

        public WrapperTransformer(string openTag, string closeTag)
        {
            OpenTag = openTag;
            CloseTag = closeTag;
        }

        public IEnumerable<string> Transform(IEnumerable<string> input)
        {
            return input.Select(text => $"{OpenTag}{text}{CloseTag}");
        }

        public int Order => 200;
    }
}