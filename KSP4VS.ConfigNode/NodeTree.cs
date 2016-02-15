using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    [DebuggerDisplay("{Name}:{Text}")]
    public class NodeTree
    {
        private List<NodeTree> subNodes = new List<NodeTree>();
        public IReadOnlyList<NodeTree> SubNodes => subNodes;
        public string Name { get; }
        public string Text { get; }

        internal NodeTree(TempNodeTree node)
        {
            Name = node.Name;
            Text = node.Text;
            subNodes.AddRange(node.subNodes);   
        }
    }
}
