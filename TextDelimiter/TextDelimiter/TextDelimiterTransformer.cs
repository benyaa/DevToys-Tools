using System;
using System.Collections.Generic;

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

        public string DelimitText(ReadOnlyMemory<char> text)
        {
            var parts = ExplodeText(text);

            foreach (var transformation in Transformations.OrderBy(t => t.Order))
            {
                parts = transformation.Transform(parts);
            }

            return string.Join(Delimiter, parts.Select(p => p.ToString()));
        }

        private IEnumerable<ReadOnlyMemory<char>> ExplodeText(ReadOnlyMemory<char> text)
        {
            char[] delimiters = ExplodeOption switch
            {
                ExplodeMode.NewLines => new[] { '\n', '\r' },
                ExplodeMode.Spaces => new[] { ' ' },
                ExplodeMode.Commas => new[] { ',' },
                ExplodeMode.Semicolons => new[] { ';' },
                _ => new[] { ' ' }
            };

            return Split(text, delimiters);
        }

        private IEnumerable<ReadOnlyMemory<char>> Split(ReadOnlyMemory<char> text, char[] delimiters)
        {
            List<ReadOnlyMemory<char>> parts = new List<ReadOnlyMemory<char>>();
            int start = 0;
            ReadOnlySpan<char> span = text.Span;

            while (start < span.Length)
            {
                int index = span.Slice(start).IndexOfAny(delimiters);
                if (index == -1)
                {
                    parts.Add(text.Slice(start));
                    break;
                }

                if (index > 0)
                {
                    parts.Add(text.Slice(start, index));
                }

                start += index + 1;
            }

            return parts;
        }


    }

}