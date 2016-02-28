using Microsoft.VisualStudio.ProjectSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KSP4VS.Deploy.Core;
using Octokit;

namespace KSP4VS.Deploy.Targets.GitHub
{
    public class GitHubDeploy : IDeployTarget, INotifyPropertyChanged
    {
        private const string OAuthTokenKey = "GitHubOAuthToken";
        private const string UsernameKey = "GitHubUsername";

        private readonly IUserSettings settings;
        private readonly IThreadHandling threadHandler;
        private readonly IProjectLockService projectLockService;
        private readonly UnconfiguredProject project;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
        public string TargetName => nameof(GitHub);

        private string branch;

        public string Branch
        {
            get { return branch; }
            set { branch = value; NotifyPropertyChanged(); }
        }

        private string versionNamePattern;

        public string VersionNamePattern
        {
            get { return versionNamePattern; }
            set { versionNamePattern = value; NotifyPropertyChanged(); }
        }

        private string repoName;

        public string RepositoryName
        {
            get { return repoName; }
            set { repoName = value; NotifyPropertyChanged(); }
        }


        public bool IsAuthorized => settings.PropertyExists(OAuthTokenKey);

        [ImportingConstructor]
        public GitHubDeploy(IUserSettings settings, UnconfiguredProject project, IProjectLockService projectLockService, IThreadHandling threadHandler)
        {
            this.project = project;
            this.projectLockService = projectLockService;
            this.threadHandler = threadHandler;
            this.settings = settings;
        }

        public async Task<bool> DeployProject()
        {
            await LoadProjectSettings();
            var credentials = new Credentials(settings.GetString(OAuthTokenKey));
            var client = new GitHubClient(new ProductHeaderValue("Kerbal Space Program for Visual Studio"))
            {
                Credentials = credentials
            };
            var name = "";
            var versionName = "";
            await threadHandler.AsyncPump.RunAsync(async () =>
            {
                using (var readLock = await projectLockService.ReadLockAsync())
                {
                    var msBuildProject = await readLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    name = msBuildProject.GetPropertyValue("Name");
                    versionName = msBuildProject.GetProperty(nameof(VersionNamePattern)).EvaluatedValue;
                }
            });
            var tag = new NewTag
            {
                Message = $"Release {versionName} of {name}",
                Tag = versionName,
                Type = TaggedType.Commit,
                Tagger = new Committer(settings.GetString(UsernameKey), "", DateTimeOffset.UtcNow)
            };

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
                    msBuildProject.SetProperty(nameof(RepositoryName), RepositoryName);
                    msBuildProject.Save();
                }
            });
        }

        public async Task LoadProjectSettings()
        {
            await threadHandler.AsyncPump.RunAsync(async () =>
            {
                using (var readLock = await projectLockService.ReadLockAsync())
                {
                    var msBuildProject = await readLock.GetProjectAsync(await project.GetSuggestedConfiguredProjectAsync());
                    Branch = msBuildProject.GetProperty(nameof(Branch))?.UnevaluatedValue ?? "master";
                    VersionNamePattern = msBuildProject.GetProperty(nameof(VersionNamePattern))?.UnevaluatedValue ?? "$(MajorVersion).$(MinorVersion)";
                    RepositoryName = msBuildProject.GetProperty(nameof(RepositoryName))?.EvaluatedValue ?? msBuildProject.GetPropertyValue("Name");
                }
            });
        }
    }
}
