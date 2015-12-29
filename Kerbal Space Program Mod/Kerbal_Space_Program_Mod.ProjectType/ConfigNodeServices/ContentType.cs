using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNodeServices
{
    static class ContentType
    {
        public const string Name = "ConfigNode";

        [Export]
        [Name(Name)]
        [DisplayName("KSP Config file")]
        [BaseDefinition("code")]
        public static ContentTypeDefinition ConfigNodeContentType;

        [Export]
        [FileExtension(".cfg")]
        [ContentType(Name)]
        public static FileExtensionToContentTypeDefinition TestFileExtensionDefinition;
    }
}
