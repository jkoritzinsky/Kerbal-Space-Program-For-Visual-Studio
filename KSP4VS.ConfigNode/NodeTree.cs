using System;
using Pegasus.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace KSP4VS.ConfigNode
{

    [DebuggerDisplay("{Name} @{Start.Position}->{End.Position}:{Text}")]
    public class NodeTree
    {
        public NodeTree Parent { get; private set; }
        private readonly LinkedList<NodeTree> subNodes = new LinkedList<NodeTree>();
        public string Name => element.Name;
        public Cursor Start => element.StartCursor;
        public Cursor End => element.EndCursor;
        public string Text { get; }
        private LexicalElement element;

        internal NodeTree(string text, LexicalElement element)
        {
            this.element = element;
            Text = text.Substring(element.StartCursor.Location, element.EndCursor.Location - element.StartCursor.Location);
        }

        internal void Add(NodeTree childNode)
        {
            subNodes.AddFirst(childNode);
            childNode.Parent = this;
        }

        public IEnumerable<NodeTree> this[string node]
        {
            get
            {
                return subNodes.Where(element => element.Name == node);
            }
        }

        public IEnumerable<NodeTree> DecendantNodesWithName(string name)
        {
            // Do a Breadth First Search to find the closest node with the right name
            var nodesToSearch = new Queue<NodeTree>(subNodes);
            while (nodesToSearch.Count != 0)
            {
                var current = nodesToSearch.Dequeue();
                if (current.Name == name)
                    yield return current;
                else
                {
                    foreach (var subNode in current.subNodes)
                    {
                        nodesToSearch.Enqueue(subNode);
                    }
                }
            }
        }

        public NodeTree FirstDecendantWithName(string name) => DecendantNodesWithName(name).FirstOrDefault();

        public int ChildrenCount => subNodes.Count;
    }
}