using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public abstract class Validator
    {
        public abstract IEnumerable<Warning> Validate(string fileName, NodeTree tree);
    }
}
