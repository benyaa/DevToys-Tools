public interface ITextTransformer
{
    IEnumerable<string> Transform(IEnumerable<string> input);

    int Order { get; }
}