namespace TextDelimiter
{
    public class TextDelimiterTransformer
    {
        public ExplodeMode ExplodeOption { get; set; }
        public string Delimiter { get; set; }
        public List<ITextTransformer> Transformations { get; set; }

        public TextDelimiterTransformer(ExplodeMode explodeOption, string delimiter, List<ITextTransformer> transformations)
        {
            ExplodeOption = explodeOption;
            Delimiter = delimiter;
            Transformations = transformations;
        }

        public string DelimitText(string text)
        {
            IEnumerable<string> parts = ExplodeText(text);

            foreach (var transformation in Transformations)
            {
                parts = transformation.Transform(parts);
            }

            return string.Join(Delimiter, parts);
        }

        private IEnumerable<string> ExplodeText(string text)
        {
            char[] delimiters = ExplodeOption switch
            {
                ExplodeMode.NewLines => new[] { '\n', '\r' },
                ExplodeMode.Spaces => new[] { ' ' },
                ExplodeMode.Commas => new[] { ',' },
                ExplodeMode.Semicolons => new[] { ';' },
                _ => new[] { ' ' }
            };

            return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }
    }

}