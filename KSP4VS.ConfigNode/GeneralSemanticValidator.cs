using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public class GeneralSemanticValidator : SemanticValidator
    {
        private const string Category = "General";
        public override IEnumerable<SemanticWarning> Validate(string fileName, NodeTree tree)
        {
            var nodeNames = tree.DecendantNodesWithName("nodeName");
            foreach (var name in nodeNames)
            {
                var flexDeclarations = name.DecendantNodesWithName("flexOrderDeclaration");
                var staticDeclaration = name.DecendantNodesWithName("staticOrderDeclaration");
                if (flexDeclarations.Any() || staticDeclaration.Any())
                {
                    var parent = name.Parent;
                    var foundConfigNodeParent = false;
                    for (; parent != null; parent = parent.Parent)
                    {
                        if (!foundConfigNodeParent)
                        {
                            if (parent.Name == "configNode")
                                foundConfigNodeParent = true;
                        }
                        else
                        {
                            if (parent.Name == "configNode")
                            {
                                yield return new SemanticWarning(fileName, name.Start, name.End, Category, "CN003");
                                break;
                            }
                        }
                    }
                }
                if (flexDeclarations.Any(decl => decl.Text != "FOR") && staticDeclaration.Any())
                    yield return new SemanticWarning(fileName, name.Start, name.End, Category, "CN001");
                if (flexDeclarations.Count(decl => decl.Text == "FOR") > 1)
                    yield return new SemanticWarning(fileName, name.Start, name.End, Category, "CN002");
                if (staticDeclaration.Count() > 1)
                    yield return new SemanticWarning(fileName, name.Start, name.End, Category, "CN001");
                if (flexDeclarations.Where(decl => decl.Text != "FOR").GroupBy(decl => decl.Text).Count() > 1)
                    yield return new SemanticWarning(fileName, name.Start, name.End, Category, "CN001");
            }
        }
    }
}
