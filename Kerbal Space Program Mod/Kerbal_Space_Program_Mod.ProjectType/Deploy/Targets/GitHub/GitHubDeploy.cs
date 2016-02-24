﻿using Microsoft.VisualStudio.ProjectSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy.Targets.GitHub
{
    [Export(typeof(IDeployTarget))]
    public class GitHubDeploy : IDeployTarget, INotifyPropertyChanged
    {
        private readonly IThreadHandling threadHandler;
        private readonly IProjectLockService projectLockService;
        private readonly UnconfiguredProject project;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
        public string TargetName => nameof(GitHub);

        public string Branch { get; set; }
        public string VersionNamePattern { get; set; }

        [ImportingConstructor]
        public GitHubDeploy(UnconfiguredProject project, IProjectLockService projectLockService, IThreadHandling threadHandler)
        {
            this.project = project;
            this.projectLockService = projectLockService;
            this.threadHandler = threadHandler;
        }

        public async Task<bool> DeployProject()
        {
            await LoadUserSettings();
            await LoadProjectSettings();
            return false;
        }

        public async Task SaveProjectSettings()
        {
            await threadHandler.AsyncPump.RunAsync(async () =>
            {
                using (var writeLock = await projectLockService.WriteLockAsync())
                {
                    await writeLock.CheckoutAsync(project.FullPath);
                    var msBuildProject = await writeLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    msBuildProject.SetProperty(nameof(GitHub) + nameof(Branch), Branch);
                    msBuildProject.SetProperty(nameof(VersionNamePattern), VersionNamePattern);
                }
            });
        }

        public Task SaveUserSettings()
        {
            throw new NotImplementedException();
        }

        public async Task LoadProjectSettings()
        {
            await threadHandler.AsyncPump.RunAsync(async () =>
            {
                using (var readLock = await projectLockService.ReadLockAsync())
                {
                    var msBuildProject = await readLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    Branch = msBuildProject.GetPropertyValue(nameof(Branch));
                    VersionNamePattern = msBuildProject.GetPropertyValue(nameof(VersionNamePattern));
                }
            });
        }

        public Task LoadUserSettings()
        {
            throw new NotImplementedException();
        }
    }
}