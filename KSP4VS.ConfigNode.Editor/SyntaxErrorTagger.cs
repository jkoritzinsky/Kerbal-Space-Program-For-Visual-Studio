using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pegasus.Common;
using Microsoft.VisualStudio.Text.Adornments;

namespace KSP4VS.ConfigNode.Editor
{
    class SyntaxErrorTagger : ITagger<ErrorTag>
    {
        private readonly BufferParser parser;
        ITextBuffer buffer;

        public SyntaxErrorTagger(BufferParser parser)
        {
            buffer = parser.Buffer;
            buffer.Changed += BufferChanged;
            this.parser = parser;
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                // If there is a change in the number of brackets, we need to re-tag everything
                if (change.OldText.Count(c => c == '}' || c == '{') != change.NewText.Count(c => c == '}' || c == '{'))
                {
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(buffer.CurrentSnapshot, new Span(0, buffer.CurrentSnapshot.Length))));
                }
                else
                {
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.After, change.NewSpan)));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = new List<ITagSpan<ErrorTag>>();
            foreach (var span in spans)
            {
                tags.AddRange(GetTagsFromElements(parser.ParsedRepresentation, span));
            }
            return tags;
        }

        private IEnumerable<ITagSpan<ErrorTag>> GetTagsFromElements(IEnumerable<LexicalElement> elements, SnapshotSpan span)
        {
            foreach (var element in elements.Where(element => element.Name.Contains("_")))
            {
                var startIndex = element.StartCursor.Location;
                var endIndex = element.EndCursor.Location;
                if (startIndex > span.End.Position)
                    continue;
                if (endIndex > span.End.Position)
                    endIndex = span.End.Position;
                var errorSpan = new Span(startIndex, endIndex - startIndex);
                if (errorSpan.IntersectsWith(span))
                {
                    var errorSnapshotSpan = new SnapshotSpan(span.Snapshot, errorSpan).TrimWhitespace().EnsureOnlyOneLine();
                    yield return new TagSpan<ErrorTag>(errorSnapshotSpan, new ErrorTag(PredefinedErrorTypeNames.SyntaxError, ErrorMessageMap.Map(element.Name)));
                }
            }
        }
    }
}
