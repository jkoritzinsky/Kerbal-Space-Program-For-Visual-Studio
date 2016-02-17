using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public class Builder
    {
        public readonly NodeTree ast;

        public Builder(string text, IEnumerable<LexicalElement> lexical)
        {
            ast = BuildAST(text, lexical.Reverse()).Item1;
        }

        private Tuple<NodeTree, int> BuildAST(string text, IEnumerable<LexicalElement> orderedTokens)
        {
            var root = orderedTokens.FirstOrDefault();
            if (root == null) return null;
            var node = new NodeTree(text, root);
            var subTokens = orderedTokens.Skip(1).TakeWhile(element => element.StartCursor.Location >= root.StartCursor.Location);
            var usedTokens = 1;
            while (subTokens.Any())
            {
                var childNode = BuildAST(text, subTokens);
                if (childNode != null)
                {
                    node.Add(childNode.Item1);
                    subTokens = subTokens.Skip(childNode.Item2);
                    usedTokens += childNode.Item2;
                }
            }
            return new Tuple<NodeTree, int>(node, usedTokens);
        }
    }
}
