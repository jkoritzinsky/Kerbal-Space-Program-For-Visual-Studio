using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode.Editor
{
    static class SnapshotSpanExtensions
    {
        public static SnapshotSpan EnsureOnlyOneLine(this SnapshotSpan snapshotSpan)
        {
            var snapshotLines = snapshotSpan.GetText().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (snapshotLines.Length == 0)
            {
                return snapshotSpan;
            }
            return new SnapshotSpan(snapshotSpan.Snapshot, snapshotSpan.Start, snapshotLines[0].Length);
        }

        public static SnapshotSpan TrimWhitespace(this SnapshotSpan snapshotSpan)
        {
            var textInSnapshot = snapshotSpan.GetText();
            var frontTrimmed = snapshotSpan.GetText().TrimStart();
            var startOffset = 0;
            if (frontTrimmed.Length < textInSnapshot.Length)
            {
                startOffset = textInSnapshot.Length - frontTrimmed.Length;
            }
            var endOffset = 0;
            var backTrimmed = snapshotSpan.GetText().TrimEnd();
            if (backTrimmed.Length < textInSnapshot.Length)
            {
                endOffset = textInSnapshot.Length - backTrimmed.Length;
            }
            var length = snapshotSpan.Length - startOffset - endOffset;
            return new SnapshotSpan(snapshotSpan.Snapshot, new Span(snapshotSpan.Start + startOffset, length >= 0 ? length : 0));
        }
    }
}
