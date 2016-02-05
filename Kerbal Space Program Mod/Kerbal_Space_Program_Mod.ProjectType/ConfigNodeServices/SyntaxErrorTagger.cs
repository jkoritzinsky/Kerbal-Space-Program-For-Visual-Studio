using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pegasus.Common;
using Microsoft.VisualStudio.Text.Adornments;

namespace KSP4VS.ConfigNodeServices
{
    class SyntaxErrorTagger : ITagger<ErrorTag>
    {
        ITextBuffer buffer;

        public SyntaxErrorTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            buffer.Changed += BufferChanged;
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.After, change.NewSpan)));
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var parser = new NodeParser(true);
            var tags = new List<ITagSpan<ErrorTag>>();
            foreach (var span in spans)
            {
                //Reparse the whole thing.  It's just must simpler
                var spanToParse = new SnapshotSpan(span.Snapshot, new Span(0, span.Snapshot.Length));
                IList<LexicalElement> lexicalElements;
                parser.Parse(spanToParse.GetText(), "Editor", out lexicalElements);
                try
                {
                    tags.AddRange(GetTagsFromElements(lexicalElements, span));
                }
                catch (FormatException)
                {
                }
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
                    var errorSnapshotSpan = new SnapshotSpan(span.Snapshot, errorSpan);
                    yield return new TagSpan<ErrorTag>(errorSnapshotSpan, new ErrorTag(PredefinedErrorTypeNames.SyntaxError));
                }
            }
        }
    }
}
