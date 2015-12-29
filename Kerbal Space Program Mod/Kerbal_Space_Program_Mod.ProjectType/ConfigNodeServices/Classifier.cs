//------------------------------------------------------------------------------
// <copyright file="Classifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Pegasus.Common;

namespace KSP4VS.ConfigNodeServices
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "Classifier" classification type.
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
                var span = GetFullNodeElement(new SnapshotSpan(e.After, change.NewSpan));
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
            }
        }

        private SnapshotSpan GetFullNodeElement(SnapshotSpan span)
        {
            var snapshot = span.Snapshot;
            var startLine = span.Start.GetContainingLine();
            var startLineNumber = startLine.LineNumber;
            if (startLineNumber == snapshot.LineCount - 1)
                --startLineNumber;
            var endLineNumber = snapshot.GetLineNumberFromPosition(span.End);
            for (var nextLineText = snapshot.GetLineFromLineNumber(startLineNumber + 1).GetText();
                                startLineNumber > 0 && !nextLineText.Contains("{");
                                nextLineText = snapshot.GetLineFromLineNumber(startLineNumber--).GetText())
                ;
            for (var lineText = snapshot.GetLineFromLineNumber(endLineNumber).GetText();
                    endLineNumber < snapshot.LineCount - 1 && !lineText.Contains("}");
                    lineText = snapshot.GetLineFromLineNumber(++endLineNumber).GetText())
                ;
            var startPoint = snapshot.GetLineFromLineNumber(startLineNumber).Start;
            var endPoint = snapshot.GetLineFromLineNumber(endLineNumber).End;
            return new SnapshotSpan(startPoint, endPoint);
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
            var fullNodeSpan = GetFullNodeElement(span);
            var parser = new InProgressNodeParser();
            try
            {
                var parseResult = parser.Parse(fullNodeSpan.GetText());
                return GenerateSpansFromTokenList(fullNodeSpan, parseResult);
            }
            catch (FormatException ex)
            {
                var tokenList = (List<Token>)((Cursor)ex.Data["cursor"])["List"];
                return GenerateSpansFromTokenList(fullNodeSpan, tokenList);
            }
        }

        private IList<ClassificationSpan> GenerateSpansFromTokenList(SnapshotSpan fullNodeSpan, List<Token> tokenList)
        {
            var result = new List<ClassificationSpan>();
            foreach (var token in tokenList)
            {
                result.Add(new ClassificationSpan(new SnapshotSpan(fullNodeSpan.Snapshot, new Span(fullNodeSpan.Start + token.Location, token.Length)), GetClassificationForToken(token.TokenType)));
            }
            return result;
        }

        private static readonly Dictionary<Token.Type, string> TokenToFormat = new Dictionary<Token.Type, string>
        {
            {Token.Type.Name, "ConfigNode.name" },
            {Token.Type.Value, "ConfigNode.value" },
            {Token.Type.Comment, "ConfigNode.comment" },
            {Token.Type.Other, "ConfigNode" }
        };


        private IClassificationType GetClassificationForToken(Token.Type tokenType)
        {
            string classificationType;
            if (!TokenToFormat.TryGetValue(tokenType, out classificationType))
                throw new ArgumentException("Unable to find classification type for " + tokenType.ToString(), nameof(tokenType));

            return classificationRegistry.GetClassificationType(classificationType);
        }
    }
}
