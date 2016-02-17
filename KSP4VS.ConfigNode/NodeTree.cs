using System;
using Pegasus.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace KSP4VS.ConfigNode
{

    [DebuggerDisplay("{Name}:{Text} @{Start}->{End}")]
    public class NodeTree
    {
        private readonly LinkedList<NodeTree> subNodes = new LinkedList<NodeTree>();
        public string Name => element.Name;
        public int Start => element.StartCursor.Location;
        public int End => element.EndCursor.Location;
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