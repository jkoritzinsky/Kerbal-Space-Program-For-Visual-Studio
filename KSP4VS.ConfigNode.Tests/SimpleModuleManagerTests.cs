using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KSP4VS.ConfigNode.Tests
{
    public class SimpleModuleManagerTests
    {
        [Fact]
        public void EditWithNameSpecParses()
        {
            var parser = new NodeParser(false);
            parser.Parse("@NODE[test]{}");
        }
    }
}
