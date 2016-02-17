using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class NodeBuilderTests : TestBase
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
            var lexicalElements = ParseAndGetElements(text);
            var builder = new Builder(text, lexicalElements);
            var ast = builder.ast;
            Assert.Equal("nodes", ast.Name);
            Assert.Equal(1, ast.ChildrenCount);
            Assert.Equal(1, ast["configNode"].Count());
            Assert.Equal("NODE", ast.FirstDecendantWithName("nodeName").Text);
            Assert.Equal("name = true", ast.FirstDecendantWithName("nameValuePair").Text);
            Assert.Equal("NESTEDNODE", ast.DecendantNodesWithName("nodeName").ElementAt(1).Text);
        }
    }
}
