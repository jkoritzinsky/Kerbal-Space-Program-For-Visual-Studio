using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode.Editor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("ConfigNode")]
    [TagType(typeof(ErrorTag))]
    class SyntaxErrorTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate () { return new SyntaxErrorTagger(buffer) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
}
