using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class NodeBuilderTests
    {
        [Fact]
        public void SimpleConfigNodeBuildTest()
        {
            var text = @"
NODE
{
    name = true
    NESTEDNODE
    {
        otherName = 5.0
    }
}
";
            var parser = new NodeParser(false);
            IList<LexicalElement> lexicalElements;
            parser.Parse(text, null, out lexicalElements);
            var builder = new Builder(text, lexicalElements);
            var ast = builder.ast;
            Assert.Equal("nodes", ast.Name);
            Assert.Equal(1, ast.SubNodes.Count);
            Assert.Equal("configNode", ast.SubNodes[0].Name);
            Assert.Equal("NODE", ast.SubNodes[0].SubNodes[0].Text);
            Assert.Equal("name = true", ast.SubNodes[0].SubNodes[1].Text);
        }
    }
}
