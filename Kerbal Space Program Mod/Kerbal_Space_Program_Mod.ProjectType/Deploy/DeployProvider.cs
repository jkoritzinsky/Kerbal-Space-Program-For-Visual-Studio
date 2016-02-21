using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace KSP4VS.Deploy
{
    [Export(typeof(IDeployProvider))]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    internal class DeployProvider : IDeployProvider
    {
        /// <summary>
        /// Provides access to the project's properties.
        /// </summary>
        [Import]
        private ProjectProperties Properties { get; set; }

        public async Task DeployAsync(CancellationToken cancellationToken, TextWriter outputPaneWriter)
        {
            // Add your custom deploy code here.  Write informational output to the outputPaneWriter.
            await Task.Yield();
        }

        public bool IsDeploySupported
        {
            get { return false; }
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }
    }
}