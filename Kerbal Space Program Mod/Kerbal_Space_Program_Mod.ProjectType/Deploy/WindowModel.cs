using Microsoft.VisualStudio.ProjectSystem;
using System.IO;
using System;
using System.Threading.Tasks;

namespace KSP4VS.Deploy
{
    internal class WindowModel
    {
        private readonly UnconfiguredProject project;

        public WindowModel(UnconfiguredProject project)
        {
            this.project = project;
        }

        public string Name => Path.GetFileNameWithoutExtension(project.FullPath);

        internal Task LoadFromProject()
        {
            return Task.CompletedTask;
        }

        internal void Save()
        {

        }
    }
}