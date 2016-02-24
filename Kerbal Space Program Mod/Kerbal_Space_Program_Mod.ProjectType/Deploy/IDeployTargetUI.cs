using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy
{
    public interface IDeployTargetUI
    {
        System.Windows.Controls.Control ProjectSettingsUI { get; }
        System.Windows.Controls.Control UserSettingsUI { get; }

        IDeployTarget DeployTarget { get; }
        
        string Name { get; }
    }
}
