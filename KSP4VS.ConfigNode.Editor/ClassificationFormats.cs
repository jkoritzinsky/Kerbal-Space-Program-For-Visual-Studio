using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSP4VS.ConfigNode.Editor
{
    static class ClassificationFormats
    {
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode")]
        [Name("ConfigNode")]
        [Order(Before = Priority.Default)]
        sealed class BaseFormat : ClassificationFormatDefinition
        {
            public BaseFormat() { }
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
            public CommentFormat() { ForegroundColor = new Color {A = 0xFF, G = 100 }; }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode.keyword")]
        [Name("ConfigNode.keyword")]
        sealed class KeywordFormat : ClassificationFormatDefinition
        {
            public KeywordFormat() { ForegroundColor = Colors.Blue; }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "ConfigNode.selector")]
        [Name("ConfigNode.selector")]
        [Order(After = Priority.Default)]
        sealed class SelectorFormat : ClassificationFormatDefinition
        {
            public SelectorFormat() { ForegroundColor = new Color { A = 0xFF, R = 0x00, G = 120, B = 120 }; }
        }
    }
}
