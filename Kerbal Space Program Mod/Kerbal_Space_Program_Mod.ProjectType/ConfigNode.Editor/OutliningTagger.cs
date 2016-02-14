using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace KSP4VS.ConfigNode.Editor
{
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        string startHide = "{";     //the characters that start the outlining region
        string endHide = "}";       //the characters that end the outlining region
        string ellipsis = "...";    //the characters that are displayed when the region is collapsed
        string hoverText = "Collapsed Region"; //the contents of the tooltip for the collapsed span
        ITextBuffer buffer;
        ITextSnapshot snapshot;
        List<Region> regions;

        public OutliningTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            snapshot = buffer.CurrentSnapshot;
            regions = new List<Region>();
            ReParse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;
            var currentRegions = regions;
            var currentSnapshot = snapshot;
            var entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            var startLineNumber = entire.Start.GetContainingLine().LineNumber;
            var endLineNumber = entire.End.GetContainingLine().LineNumber;
            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    //the region starts at the beginning of the "{", and goes until the *end* of the line that contains the "}".
                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.Start + region.StartOffset,
                        endLine.End),
                        new OutliningRegionTag(false, false, ellipsis, hoverText));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != buffer.CurrentSnapshot)
                return;
            ReParse();
        }

        void ReParse()
        {
            var newSnapshot = buffer.CurrentSnapshot;
            var newRegions = new List<Region>();

            //keep the current (deepest) partial region, which will have
            // references to any parent partial regions.
            PartialRegion currentRegion = null;

            foreach (var line in newSnapshot.Lines)
            {
                var regionStart = -1;
                var text = line.GetText();

                //lines that contain a "[" denote the start of a new region.
                if ((regionStart = text.IndexOf(startHide, StringComparison.Ordinal)) != -1)
                {
                    var currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                    int newLevel;
                    if (!TryGetLevel(text, regionStart, out newLevel))
                        newLevel = currentLevel + 1;

                    //levels are the same and we have an existing region;
                    //end the current region and start the next
                    if (currentLevel == newLevel && currentRegion != null)
                    {
                        newRegions.Add(new Region
                        {
                            Level = currentRegion.Level,
                            StartLine = currentRegion.StartLine,
                            StartOffset = currentRegion.StartOffset,
                            EndLine = line.LineNumber
                        });

                        currentRegion = new PartialRegion
                        {
                            Level = newLevel,
                            StartLine = line.LineNumber,
                            StartOffset = regionStart,
                            PartialParent = currentRegion.PartialParent
                        };
                    }
                    //this is a new (sub)region
                    else
                    {
                        currentRegion = new PartialRegion
                        {
                            Level = newLevel,
                            StartLine = line.LineNumber,
                            StartOffset = regionStart,
                            PartialParent = currentRegion
                        };
                    }
                }
                //lines that contain "]" denote the end of a region
                else if ((regionStart = text.IndexOf(endHide, StringComparison.Ordinal)) != -1)
                {
                    var currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                    int closingLevel;
                    if (!TryGetLevel(text, regionStart, out closingLevel))
                        closingLevel = currentLevel;

                    //the regions match
                    if (currentRegion != null &&
                        currentLevel == closingLevel)
                    {
                        newRegions.Add(new Region
                        {
                            Level = currentLevel,
                            StartLine = currentRegion.StartLine,
                            StartOffset = currentRegion.StartOffset,
                            EndLine = line.LineNumber
                        });

                        currentRegion = currentRegion.PartialParent;
                    }
                }
            }

            //determine the changed span, and send a changed event with the new spans
            var oldSpans =
                new List<Span>(regions.Select(r => AsSnapshotSpan(r, snapshot)
                    .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                    .Span));
            var newSpans =
                    new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            var oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            var newSpanCollection = new NormalizedSpanCollection(newSpans);

            //the changed regions are regions that appear in one set or the other, but not both.
            var removed =
            NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            var changeStart = int.MaxValue;
            var changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            snapshot = newSnapshot;
            regions = newRegions;

            if (changeStart <= changeEnd)
            {
                var snap = snapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                        new SnapshotSpan(snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        static bool TryGetLevel(string text, int startIndex, out int level)
        {
            level = -1;
            if (text.Length > startIndex + 3)
            {
                if (int.TryParse(text.Substring(startIndex + 1), out level))
                    return true;
            }

            return false;
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ? startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }

        class PartialRegion
        {
            public int StartLine { get; set; }
            public int StartOffset { get; set; }
            public int Level { get; set; }
            public PartialRegion PartialParent { get; set; }
        }

        class Region : PartialRegion
        {
            public int EndLine { get; set; }
        }
    }
}
