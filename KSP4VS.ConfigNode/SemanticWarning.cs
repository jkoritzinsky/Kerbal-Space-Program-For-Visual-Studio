using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNode
{
    public struct SemanticWarning
    {
        public Cursor Start { get; }
        public Cursor End { get; }
        public string SubCategory { get; }
        public string WarningCode { get; }
        public string Message { get; }
        public string File { get; }
        public SemanticWarning(string file, Cursor start, Cursor end, string subcategory, string warningCode, string message = null)
            : this()
        {
            Start = start;
            End = end;
            SubCategory = subcategory;
            WarningCode = warningCode;
            Message = message ?? ErrorMessageMap.Map(WarningCode) ?? string.Empty;
            File = file;
        }
    }
}
