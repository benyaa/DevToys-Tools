
namespace TextDelimiter.Transformations
{
    public class AttackTheClonesTransformer : ITextTransformer
    {
        public IEnumerable<string> Transform(IEnumerable<string> input) => input.Distinct();

        public int Order => 1000;
    }
}