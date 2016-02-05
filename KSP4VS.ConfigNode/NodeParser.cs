using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public partial class NodeParser
    {
        private bool editorMode;
        public NodeParser(bool editorMode)
        {
            this.editorMode = editorMode;
        }
    }
}
