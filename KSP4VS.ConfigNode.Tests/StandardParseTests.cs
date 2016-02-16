using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class StandardParseTests : TestBase
    {
        [Fact]
        public void EmptyConfigNodeNoNewlineParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("PART {}"), IsError);
        }

        [Fact]
        public void EmptyConfigNodeWithNewlineParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("PART\r\n{}"), IsError);
        }

        [Fact]
        public void ConfigNodeWithValueEntriesParses()
        {
            Assert.DoesNotContain(ParseAndGetElements(@"
                NODE
                {
                name = value
                }"), IsError);
        }

        [Fact]
        public void TopLevelValueEntryParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("name = value"), IsError);
        }

        [Fact]
        public void ConfigNodeContainingConfigNodeParses()
        {
            Assert.DoesNotContain(ParseAndGetElements(@"
                NODE
                {
                    SUBNODE
                    {
                        name = value
                    }
                }"), IsError);

        }

        [Fact]
        public void TwoTopLevelConfigNodesParses()
        {
            Assert.DoesNotContain(ParseAndGetElements(@"
                NODE {}
                NODE {}"), IsError);
        }


        [Fact]
        public void NameWithNumberHasErrorToken()
        {
            Assert.Contains(ParseAndGetElements("name2 = value\r\n"), (element) => element.Name == "name_InvalidChar");
        }
    }
}
