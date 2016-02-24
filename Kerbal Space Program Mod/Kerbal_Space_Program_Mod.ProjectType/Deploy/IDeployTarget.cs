﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy
{
    public interface IDeployTarget
    {
        string TargetName { get; }

        Task LoadProjectSettings();
        Task LoadUserSettings();
        Task SaveProjectSettings();
        Task SaveUserSettings();

        Task<bool> DeployProject();
    }
}
