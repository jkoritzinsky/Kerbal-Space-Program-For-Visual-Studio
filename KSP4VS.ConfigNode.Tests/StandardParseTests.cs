using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class StandardParseTests
    {
        [Fact]
        public void EmptyConfigNodeNoNewlineParses()
        {
            var parser = new NodeParser(false);
            parser.Parse("PART {}");
        }

        [Fact]
        public void EmptyConfigNodeWithNewlineParses()
        {
            var parser = new NodeParser(false);
            parser.Parse("PART\r\n{ }");
        }

        [Fact]
        public void ConfigNodeWithValueEntriesParses()
        {
            var parser = new NodeParser(false);
            parser.Parse(@"
                NODE
                {
                name = value
                }");
        }

        [Fact]
        public void TopLevelValueEntryParses()
        {
            var parser = new NodeParser(false);
            parser.Parse("name = value");
        }

        [Fact]
        public void ConfigNodeContainingConfigNodeParses()
        {
            var parser = new NodeParser(false);
            parser.Parse(@"
                NODE
                {
                    SUBNODE
                    {
                        name = value
                    }
                }");
        }

        [Fact]
        public void TwoTopLevelConfigNodesParses()
        {
            var parser = new NodeParser(false);
            parser.Parse(@"
                NODE {}
                NODE {}");
        }
    }
}
