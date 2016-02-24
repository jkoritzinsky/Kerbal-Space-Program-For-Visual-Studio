using Microsoft.VisualStudio.ProjectSystem;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KSP4VS.Deploy
{
    internal class WindowModel : INotifyPropertyChanged
    {
        public IEnumerable<IDeployTargetUI> TargetUIs { get; }
        private readonly IProjectLockService lockService;
        private readonly IThreadHandling threadHandling;
        private readonly UnconfiguredProject project;
        private IDeployTargetUI selectedTarget;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public WindowModel(UnconfiguredProject project, IEnumerable<IDeployTargetUI> targetUIs, IProjectLockService lockService, IThreadHandling threadHandling)
        {
            this.project = project;
            this.threadHandling = threadHandling;
            this.lockService = lockService;
            TargetUIs = targetUIs;
        }

        public string Name => Path.GetFileNameWithoutExtension(project.FullPath);

        public IDeployTargetUI SelectedTarget
        {
            get
            {
                return selectedTarget;
            }
            set
            {
                selectedTarget = value;
                threadHandling.AsyncPump.Run(() => 
                    value.DeployTarget.LoadProjectSettings());
                NotifyPropertyChanged();
            }
        }

        internal async Task LoadFromProject()
        {
            await threadHandling.AsyncPump.RunAsync(async () =>
            {
                using (var readLock = await lockService.ReadLockAsync())
                {
                    var msBuildProject = await readLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    var selectedTargetNameProperty = msBuildProject.GetProperty("SelectedDeployTarget");
                    if (selectedTargetNameProperty != null && !string.IsNullOrEmpty(selectedTargetNameProperty.EvaluatedValue))
                    {
                        await threadHandling.SwitchToUIThread();
                        SelectedTarget = TargetUIs.Single(ui => ui.Name == selectedTargetNameProperty.EvaluatedValue);
                    }
                }
            });
            if (SelectedTarget != null)
            {

                await SelectedTarget.DeployTarget.LoadProjectSettings(); 
            }
        }

        internal async Task Save()
        {
            await threadHandling.AsyncPump.RunAsync(async () =>
            {
                using (var writeLock = await lockService.WriteLockAsync())
                {
                    await writeLock.CheckoutAsync(project.FullPath);
                    var msBuildProject = await writeLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    msBuildProject.SetProperty("SelectedDeployTarget", SelectedTarget?.Name ?? "");
                    msBuildProject.Save();
                }
            });
            if (SelectedTarget != null)
            {
                await SelectedTarget.DeployTarget.SaveProjectSettings(); 
            }
        }
    }
}