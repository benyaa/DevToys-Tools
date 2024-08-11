namespace TextDelimiter.Transformations
{
    public class TidyUpTransformer : ITextTransformer
    {
        public IEnumerable<ReadOnlyMemory<char>> Transform(IEnumerable<ReadOnlyMemory<char>> input)
        {
            foreach (var memory in input)
            {
                var span = memory.Span;
                var cleanedMemory = new List<char>();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == '\r' || span[i] == '\n')
                    {
                        continue;
                    }
                    cleanedMemory.Add(span[i]);
                }

                yield return new ReadOnlyMemory<char>([.. cleanedMemory]);
            }
        }

        public int Order => 100;
    }
}
