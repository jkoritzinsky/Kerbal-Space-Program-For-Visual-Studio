using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNodeServices
{
    static class ClassificationTypes
    {
        [Export]
        [Name("ConfigNode")]
        [BaseDefinition("code")]
        internal static ClassificationTypeDefinition BaseClassificationDefinition;

        [Export]
        [Name("ConfigNode.name")]
        [BaseDefinition("ConfigNode")]
        internal static ClassificationTypeDefinition NameDefinition;

        [Export]
        [Name("ConfigNode.value")]
        [BaseDefinition("ConfigNode")]
        internal static ClassificationTypeDefinition ValueDefinition;

        [Export]
        [Name("ConfigNode.comment")]
        [BaseDefinition("ConfigNode")]
        internal static ClassificationTypeDefinition CommentDefinition;

        [Export]
        [Name("ConfigNode.selector")]
        [BaseDefinition("ConfigNode")]
        internal static ClassificationTypeDefinition SelectorDefinition;

        [Export]
        [Name("ConfigNode.nameSelector")]
        [BaseDefinition("ConfigNode.name")]
        [BaseDefinition("ConfigNode.selector")]
        internal static ClassificationTypeDefinition NameSelectorDefinition;
    }
}
