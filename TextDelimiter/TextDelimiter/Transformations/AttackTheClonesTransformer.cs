
namespace TextDelimiter.Transformations
{
    public class AttackTheClonesTransformer : ITextTransformer
    {
        public IEnumerable<ReadOnlyMemory<char>> Transform(IEnumerable<ReadOnlyMemory<char>> input)
        {
            var distinctSet = new HashSet<string>();
            foreach (var memory in input)
            {
                if (distinctSet.Add(memory.ToString()))
                {
                    yield return memory;
                }
            }
        }

        public int Order => 1000;
    }
}