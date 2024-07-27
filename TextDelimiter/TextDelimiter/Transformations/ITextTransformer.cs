public interface ITextTransformer
{
    IEnumerable<ReadOnlyMemory<char>> Transform(IEnumerable<ReadOnlyMemory<char>> input);

    int Order { get; }
}
