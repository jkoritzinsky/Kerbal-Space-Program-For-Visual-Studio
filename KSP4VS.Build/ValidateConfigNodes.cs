using KSP4VS.ConfigNode;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KSP4VS.Build
{
    public sealed class ValidateConfigNodes : Task
    {
        [Required]
        public ITaskItem[] Files { get; set; }

        public bool WarningsAsErrors { get; set; }

        public bool ContinueOnError { get; set; }

        public override bool Execute()
        {
            var parser = new NodeParser();
            var validator = new GeneralSemanticValidator();
            foreach (var file in Files)
            {
                var nodes = File.ReadAllText(file.ItemSpec);
                IList<LexicalElement> tokens;
                parser.Parse(nodes, file.ItemSpec, out tokens);
                foreach (var token in tokens.Where(token => token.Name.Contains("_")))
                {
                    LogError(new Warning(file.ItemSpec, token.StartCursor, token.EndCursor, "Syntax", token.Name)); 
                }
                var ast = new Builder(nodes, tokens).ast;
                var warnings = validator.Validate(file.ItemSpec, ast);
                foreach (var warning in warnings)
                {
                    LogWarning(warning);
                }
            }
            return Log.HasLoggedErrors && !ContinueOnError;
        }

        private void LogError(Warning warning)
        {
            Log.LogError(warning.SubCategory, warning.WarningCode, "", warning.File, warning.Start.Line,
                warning.Start.Column, warning.End.Line, warning.End.Column, warning.Message);
        }

        private void LogWarning(Warning warning)
        {
            if (!WarningsAsErrors)
            {

                Log.LogWarning(warning.SubCategory, warning.WarningCode, "", warning.File, warning.Start.Line,
                    warning.Start.Column, warning.End.Line, warning.End.Column, warning.Message);
            }
            else
            {
                LogError(warning);
            }
        }
    }
}
