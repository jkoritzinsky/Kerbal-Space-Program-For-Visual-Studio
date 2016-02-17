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
            Assert.Contains(ParseAndGetElements("@NODE:HAS[Invalid Name] {}"), element => element.Name == "MMnamePattern_InvalidChar");
        }

        [Fact]
        public void NestedHasStatementParses()
        {
            var tokens = ParseAndGetElements("@NODE:HAS[@Subnode[]:HAS[~Subproperty[]]] {}");
            Assert.DoesNotContain(tokens, IsError);
            Assert.NotEqual(1, tokens.Count);
        }

        [Fact]
        public void HasFailsToParseOnValue()
        {
            var tokens = ParseAndGetElements("@name:HAS[!@Subnode[]] = value\n");
            Assert.Contains(tokens, token => token.Name == "name_InvalidChar");
        }

        [Fact]
        public void MultiNestedHasExampleFromMMWikiParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@PART[*]:HAS[@RESOURCE[MonoPropellant]:HAS[#maxAmount[750]]] {}"), IsError);
        }

        [Fact]
        public void BeforeOrderOnTopLevelParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:BEFORE[Test] {}"), IsError);
        }

        [Fact]
        public void AfterOrderOnTopLevelParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:AFTER[Test] {}"), IsError);
        }

        [Fact]
        public void ForOrderOnTopLevelParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:FOR[Test] {}"), IsError);
        }

        [Fact]
        public void FinalOrderOnTopLevelParses()
        {
            Assert.DoesNotContain(ParseAndGetElements("@NODE:FINAL {}"), IsError);
        }

        [Fact]
        public void OrderingOnNonTopLevelFails()
        {
            var tokens = ParseAndGetElements(@"
                @NODE
                {
                    @NODE:BEFORE[Test] {}
                }");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "name_InvalidChar");           
        }

        [Fact]
        public void MultipleOrderingItemsOnTopLevelParses()
        {
            var tokens = ParseAndGetElements("@NODE:BEFORE[Mod1]:FOR[Mod2]:AFTER[Mod3] {}");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void MultiplicationCorrectlyParses()
        {
            var tokens = ParseAndGetElements("@name *= 2\n");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void AdditionCorrectlyParses()
        {
            var tokens = ParseAndGetElements("@name += 2\n");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void ValueEntryChangeParses()
        {
            var tokens = ParseAndGetElements("@name = 2\n");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void OrderedValueEntryChangeParses()
        {
            var tokens = ParseAndGetElements("@name,2 = 2\n");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void NegativeOrderedValueEntryChangeParses()
        {
            var tokens = ParseAndGetElements("@name,-2 = 2\n");
            Assert.NotEqual(1, tokens.Count);
            Assert.DoesNotContain(tokens, IsError);
        }

        [Fact]
        public void RelativeVariablePathParses()
        {
            var tokens = ParseAndGetElements("@name = #$../../node$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void TopLevelNodeVariablePath()
        {
            var tokens = ParseAndGetElements("@name = #$@PART[name]/property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void SameLevelValuePath()
        {
            var tokens = ParseAndGetElements("@name = #$property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void TopLevelNodeWithSubNodeVariablePath()
        {
            var tokens = ParseAndGetElements("@name = #$@PART[name]/@MODULE[ModuleName]/property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void RelativeAndNodeWithSubNodeVariablePath()
        {
            var tokens = ParseAndGetElements("@name = #$../@PART[name]/@MODULE[ModuleName]/property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }
        [Fact]
        public void RelativeAndNodeWithOrderedSubNodeVariablePath()
        {
            var tokens = ParseAndGetElements("@name = #$../@PART[name]/@MODULE,2[ModuleName]/property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void RelativeAndNodeWithSubNodeVariablePathWithStartSlash()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void VariableListParses()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/property$ #$property$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void CommaDelimitedVariableSearchParses()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/csv[3]$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void SpaceDelimitedVariableSearchParses()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/csv[3, ]$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void OrderedSpaceDelimitedVariableSearchParses()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/csv,4[3, ]$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }

        [Fact]
        public void CloseBracketDelimitedVariableSearchParses()
        {
            var tokens = ParseAndGetElements("@name = #$/@MODULE[name]/@SUBNode[ModuleName]/csv[3,]]$ \n");
            Assert.NotEqual(1, tokens.Count);
            Assert.Contains(tokens, element => element.Name == "variable");
        }
    }
}
