//------------------------------------------------------------------------------
// <copyright file="Classifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Pegasus.Common;

namespace KSP4VS.ConfigNode.Editor
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "ConfigNode" classification type.
    /// </summary>
    internal class Classifier : IClassifier
    {
        private ITextBuffer buffer;
        private IClassificationTypeRegistryService classificationRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Classifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        public Classifier(ITextBuffer buffer, IClassificationTypeRegistryService classificationRegistry)
        {
            this.buffer = buffer;
            this.buffer.Changed += BufferChanged;
            this.classificationRegistry = classificationRegistry;
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(e.After, change.NewSpan)));
            }
        }

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            //Reparse the whole thing.  It's just must simpler
            var spanToParse = new SnapshotSpan(span.Snapshot, new Span(0, span.Snapshot.Length));
            var parser = new NodeParser(true);
            IList<LexicalElement> lexicalElements;
            try
            {
                parser.Parse(spanToParse.GetText(), "Editor", out lexicalElements);
                return GenerateSpansFromLexicalElements(spanToParse, span, lexicalElements);
            }
            catch (FormatException)
            {
                return new List<ClassificationSpan> { new ClassificationSpan(span, classificationRegistry.GetClassificationType("ConfigNode")) };
            }
        }

        private IList<ClassificationSpan> GenerateSpansFromLexicalElements(SnapshotSpan span, SnapshotSpan target, IList<LexicalElement> lexicalElements)
        {
            var classifications = new List<ClassificationSpan>();
            foreach (var element in lexicalElements.Where(element => ElementInSpan(span, target, element)))
            {
                classifications.Add(new ClassificationSpan(
                    new SnapshotSpan(span.Snapshot, 
                                    new Span(span.Start.Position + element.StartCursor.Location,
                                                element.EndCursor.Location - element.StartCursor.Location)), GetClassificationForLexicalElement(element)));
            }
            return classifications;
        }
        
        private static readonly Dictionary<string, string> LexicalNameToFormat = new Dictionary<string, string>
        {
            {"name", "ConfigNode.name" },
            { "name_noEq", "ConfigNode.name.error" },
            {"name_InvalidChar", "ConfigNode.name.error" },
            { "string", "ConfigNode.value" },
            { "comment", "ConfigNode.comment" },
            { "bool", "ConfigNode.keyword" }
        };

        private bool ElementInSpan(SnapshotSpan span, SnapshotSpan target, LexicalElement element)
        {

            var classificationSpan = new Span(span.Start.Position + element.StartCursor.Location,
                                            element.EndCursor.Location - element.StartCursor.Location);
            return classificationSpan.IntersectsWith(target);
        }

        private IClassificationType GetClassificationForLexicalElement(LexicalElement element)
        {
            string classificationType;
            if (LexicalNameToFormat.ContainsKey(element.Name))
                classificationType = LexicalNameToFormat[element.Name];
            else
                classificationType = "ConfigNode";
            return classificationRegistry.GetClassificationType(classificationType);
        }
    }
}
