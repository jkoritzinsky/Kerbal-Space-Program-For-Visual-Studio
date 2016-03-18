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
        public void NameWithNumberIsValid()
        {
            Assert.DoesNotContain(ParseAndGetElements("name2 = value\r\n"), IsError);
        }

        [Fact]
        public void NameWithInvalidSymbolInMiddleIsError()
        {
            Assert.Contains(ParseAndGetElements("name*name = value\r\n"), IsError);
        }

        [Fact]
        public void SpaceSeperatedNumListParsesAsSpaceList()
        {
            Assert.Contains(ParseAndGetElements("name = 1 2.5 -3 -4.3\r\n"), element => element.Name == "numberListSpace");
        }

        [Fact]
        public void CommaSeperatedNumListParsesAsCommaList()
        {
            Assert.Contains(ParseAndGetElements("name = -1,2.5, 3, 4, -5.7\r\n"), element => element.Name == "numberListComma");
        }
    }
}
