using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode.Tests
{
    public class TestBase
    {
        protected static IList<LexicalElement> ParseAndGetElements(string subject)
        {
            var parser = new NodeParser();
            IList<LexicalElement> elements;
            parser.Parse(subject, null, out elements);
            return elements;
        }

        protected static bool IsError(LexicalElement element) => element.Name.Contains("_");
    }
}
