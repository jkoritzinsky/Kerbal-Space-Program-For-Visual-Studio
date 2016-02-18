using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace KSP4VS.ConfigNode.Editor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("ConfigNode")]
    [TagType(typeof(ErrorTag))]
    class SemanticErrorTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() => new SemanticErrorTagger(buffer.GetBufferParser()));
        }
    }
}
