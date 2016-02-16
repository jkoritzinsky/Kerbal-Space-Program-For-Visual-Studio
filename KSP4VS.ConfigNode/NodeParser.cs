using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public partial class NodeParser
    {
        private static bool IsTopLevelNode(Cursor state)
        {
            ListNode<LexicalElement> head = state["_lexical"];
            var lexicalList = head.ToList();
            return !lexicalList.Any(element => element.Name.Contains(nameof(nodeName)));
        }
    }
}
