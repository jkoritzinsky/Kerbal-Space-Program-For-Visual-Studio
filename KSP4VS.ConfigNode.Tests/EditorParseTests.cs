using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class EditorParseTests
    {
        [Fact]
        public void NameWithNumberWorksOnlyInEditor()
        {
            var editorParser = new NodeParser(true);
            var strictParser = new NodeParser(false);
            var testString = "name2 = value";
            Assert.Throws<FormatException>(() => strictParser.Parse(testString));
            editorParser.Parse(testString);
        }
    }
}
