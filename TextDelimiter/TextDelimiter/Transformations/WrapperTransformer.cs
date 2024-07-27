namespace TextDelimiter.Transformations
{
    public class WrapperTransformer : ITextTransformer
    {
        public readonly ReadOnlyMemory<char> OpenTag;
        public readonly ReadOnlyMemory<char> CloseTag;

        public WrapperTransformer(string openTag, string closeTag)
        {
            OpenTag = openTag.AsMemory();
            CloseTag = closeTag.AsMemory();
        }

        public IEnumerable<ReadOnlyMemory<char>> Transform(IEnumerable<ReadOnlyMemory<char>> input)
        {
            foreach (var memory in input)
            {
                int totalLength = OpenTag.Length + memory.Length + CloseTag.Length;
                char[] result = new char[totalLength];

                OpenTag.Span.CopyTo(result);
                memory.Span.CopyTo(result.AsSpan(OpenTag.Length));
                CloseTag.Span.CopyTo(result.AsSpan(OpenTag.Length + memory.Length));

                yield return result;
            }
        }

        public int Order => 200;
    }
}