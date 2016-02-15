using Microsoft.VisualStudio.Text;
using Pegasus.Common;
using System;
using System.Collections.Generic;

namespace KSP4VS.ConfigNode.Editor
{
    public class BufferParser
    {
        public ITextBuffer Buffer { get; }
        private IList<LexicalElement> parsedRepresentation;
        public IList<LexicalElement> ParsedRepresentation => parsedRepresentation;
        private Lazy<NodeTree> ast;
        public NodeTree AST => ast.Value;

        private readonly NodeParser parser;

        public BufferParser(ITextBuffer text)
        {
            Buffer = text;
            parser = new NodeParser(true);
            text.Changed += Text_Changed;
            Reparse();
        }

        private void Text_Changed(object sender, TextContentChangedEventArgs e)
        {
            Reparse();
        }

        private void Reparse()
        {
            var bufferText = Buffer.CurrentSnapshot.GetText();
            try
            {

                parser.Parse(bufferText, "Editor", out parsedRepresentation);
                ast = new Lazy<NodeTree>(() => new Builder(bufferText, parsedRepresentation).ast);
            }
            catch (FormatException)
            {
                parsedRepresentation = new List<LexicalElement>();
                ast = new Lazy<NodeTree>(() => null);
            }
        }
    }
}