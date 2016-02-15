using Microsoft.VisualStudio.Text;
using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode.Editor
{
    public static class TextBuffer
    {
        public static BufferParser GetBufferParser(this ITextBuffer text) => text.Properties.GetOrCreateSingletonProperty(() => new BufferParser(text));
    }
}
