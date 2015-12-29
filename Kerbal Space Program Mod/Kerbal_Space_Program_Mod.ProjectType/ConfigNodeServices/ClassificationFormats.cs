using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSP4VS.ConfigNodeServices
{
    static class ClassificationFormats
    {
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode")]
        [Name("ConfigNode")]
        sealed class BaseFormat : ClassificationFormatDefinition
        {
            public BaseFormat() { ForegroundColor = Colors.Black; }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode.name")]
        [Name("ConfigNode.name")]
        sealed class NameFormat : ClassificationFormatDefinition
        {
            public NameFormat() { ForegroundColor = Colors.Maroon; }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode.value")]
        [Name("ConfigNode.value")]
        sealed class ValueFormat : ClassificationFormatDefinition
        {
            public ValueFormat() { IsItalic = true; }
        }


        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode.comment")]
        [Name("ConfigNode.comment")]
        sealed class CommentFormat : ClassificationFormatDefinition
        {
            public CommentFormat() { ForegroundColor = Colors.LightGreen; }
        }
    }
}
