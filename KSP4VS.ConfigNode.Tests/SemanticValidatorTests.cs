using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class SemanticValidatorTests : TestBase
    {
        [Fact]
        public void SemanticValidatorReturnsNoErrorsOnValidInput()
        {
            var text = "@NODE:FOR[MyMod] {}";
            var tokens = ParseAndGetElements(text);
            var ast = new Builder(text, tokens).ast;
            var validator = new GeneralSemanticValidator();
            Assert.False(validator.Validate("Test", ast).Any());
        }

        [Fact]
        public void MultipleForDeclarationsReturnsOneError()
        {
            var text = "@NODE:FOR[MyMod]:FOR[MyOtherMod] {}";
            var tokens = ParseAndGetElements(text);
            var ast = new Builder(text, tokens).ast;
            var validator = new GeneralSemanticValidator();
            var errors = validator.Validate("test", ast).ToList();
            Assert.Equal("CN002", errors[0].WarningCode);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void FirstAndFinalDeclarationsReturnsOneError()
        {
            var text = "@NODE:FIRST:FINAL {}";
            var tokens = ParseAndGetElements(text);
            var ast = new Builder(text, tokens).ast;
            var validator = new GeneralSemanticValidator();
            var errors = validator.Validate("test", ast).ToList();
            Assert.Equal("CN001", errors[0].WarningCode);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void BeforeAndAfterDeclarationsReturnsOneError()
        {
            var text = "@NODE:BEFORE[Mod]:AFTER[Mod] {}";
            var tokens = ParseAndGetElements(text);
            var ast = new Builder(text, tokens).ast;
            var validator = new GeneralSemanticValidator();
            var errors = validator.Validate("test", ast).ToList();
            Assert.Equal("CN001", errors[0].WarningCode);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void FirstAndAfterDeclarationsReturnsOneError()
        {
            var text = "@NODE:FIRST:AFTER[Mod] {}";
            var tokens = ParseAndGetElements(text);
            var ast = new Builder(text, tokens).ast;
            var validator = new GeneralSemanticValidator();
            var errors = validator.Validate("test", ast).ToList();
            Assert.Equal("CN001", errors[0].WarningCode);
            Assert.Equal(1, errors.Count);
        }
    }
}
