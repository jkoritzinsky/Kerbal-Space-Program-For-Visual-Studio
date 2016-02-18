using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace KSP4VS.ConfigNode.Editor
{
    public class SemanticErrorTagger : ITagger<ErrorTag>
    {
        private readonly BufferParser parser;

        private List<SemanticValidator> validators = new List<SemanticValidator>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public SemanticErrorTagger(BufferParser parser)
        {
            this.parser = parser;
            validators.Add(new GeneralSemanticValidator());
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return GetTagsFromAst(parser.AST);
        }

        private IEnumerable<ITagSpan<ErrorTag>> GetTagsFromAst(NodeTree ast)
        {
            foreach (var validator in validators)
            {
                foreach (var error in validator.Validate("", ast))
                {
                    yield return new TagSpan<ErrorTag>(new SnapshotSpan(parser.Buffer.CurrentSnapshot,
                            new Span(error.Start.Location, error.End.Location- error.Start.Location)),
                        new ErrorTag(PredefinedErrorTypeNames.CompilerError, error.Message));
                }
            }
        }
    }
}
