using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class ModuleManagerTests : TestBase
    {
        [Fact]
        public void EditWithNameSpecParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE[test]{}"), IsError);
        }

        [Fact]
        public void EditWithValidHasStatementParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:HAS[@MODULE[name]] {}"), IsError);
        }

        [Fact]
        public void MultipleValidHasStatementParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:HAS[@MODULE[name]]:HAS[~TechRequired[]] {}"), IsError);
        }

        [Fact]
        public void InvalidHasStatementParsesWithErrorToken()
        {
            Assert.Contains(ParseAndGetElements("@NODE:HAS[Invalid Name] = value \r\n"), element => element.Name == "MMnamePattern_InvalidChar");
        }

        [Fact]
        public void NestedHasStatementParses()
        {
            var tokens = ParseAndGetElements("@NODE:HAS[@Subnode[]:HAS[~Subproperty[]]] = value");
            Assert.DoesNotContain(tokens, IsError);
        }
    }
}
