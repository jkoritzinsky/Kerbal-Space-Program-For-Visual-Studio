using System;
using Pegasus.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KSP4VS.ConfigNode
{
    internal class TempNodeTree
    {
        public readonly LinkedList<NodeTree> subNodes = new LinkedList<NodeTree>();
        public string Name { get; }
        public string Text { get; }

        internal TempNodeTree(string text, LexicalElement element)
        {
            Name = element.Name;
            Text = text.Substring(element.StartCursor.Location, element.EndCursor.Location - element.StartCursor.Location);
        }

        internal void Add(NodeTree childNode)
        {
            subNodes.AddFirst(childNode);
        }
    }
}