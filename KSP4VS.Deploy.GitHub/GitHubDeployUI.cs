using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using KSP4VS.Deploy.Core;

namespace KSP4VS.Deploy.Targets.GitHub
{
    public class GitHubDeployUI : IDeployTargetUI
    {
        [ImportingConstructor]
        public GitHubDeployUI([ImportMany] IEnumerable<IDeployTarget> deployTargets)
        {
            DeployTarget = deployTargets.Single(target => target.TargetName == Name);
            ProjectSettingsUI = new GitHubProject { DataContext = DeployTarget };
        }

        public IDeployTarget DeployTarget { get; }

        public string Name => nameof(GitHub);

        public System.Windows.Controls.Control UserSettingsUI
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public System.Windows.Controls.Control ProjectSettingsUI { get; }
    }
}
