using System;
using System.Collections.Immutable;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace KSP4VS.Deploy.Core
{
    /// <summary>
    /// Provides implementation for handling commands.
    /// </summary>
    // TODO: If your implementation is async, consider using IAsyncCommandGroupHandler instead of ICommandGroupHandler
    [ExportCommandGroup("f61a30ca-328c-4a2c-b394-f5f4a5eefcd4")]
    [AppliesTo("Kerbal_Space_Program_Mod")]
    internal class CommandGroupHandler : IAsyncCommandGroupHandler
    {
        private readonly IEnumerable<IDeployTargetUI> targetUIs;
        private readonly IProjectLockService lockService;
        private UnconfiguredProject project;
        private IThreadHandling threadHandler;

        private IVsUIShell uiShell;
        private IVsRunningDocumentTable docTable;

        [ImportMany(ExportContractNames.VsTypes.IVsProject, typeof(IVsProject))]
        internal OrderPrecedenceImportCollection<IVsHierarchy> ProjectHierarchies { get; }

        internal IVsHierarchy ProjectHierarchy => ProjectHierarchies.Single().Value;

        [ImportingConstructor]
        private CommandGroupHandler(UnconfiguredProject project, IThreadHandling threadHandler, IProjectLockService lockService, 
            [ImportMany] IEnumerable<IDeployTargetUI> targetUIs, [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.threadHandler = threadHandler;
            threadHandler.VerifyOnUIThread();
            this.project = project;
            uiShell = (IVsUIShell)serviceProvider.GetService(typeof(IVsUIShell));
            docTable = (IVsRunningDocumentTable)serviceProvider.GetService(typeof(IVsRunningDocumentTable));

            ProjectHierarchies = new OrderPrecedenceImportCollection<IVsHierarchy>(projectCapabilityCheckProvider: project);
            this.lockService = lockService;
            this.targetUIs = targetUIs;
        }

        /// <summary>
        /// Check if a specific command is supported and enabled.
        /// </summary>
        /// <param name="nodes">The project nodes being queried.</param>
        /// <param name="commandId">The command ID.</param>
        /// <param name="focused">A value indicating whether <paramref name="nodes"/> or the project have the user focus.
        ///     A value of <c>false</c> indicates this command is being routed through the application in search of command handlers to process a command that the focused UI did not handle.</param>
        /// <param name="commandText">The default caption of the command that is displayed to the user.  <c>null</c> to allow the default caption to be used.</param>
        /// <param name="progressiveStatus">The query result thus far (as default, or as handed off from previous handler).</param>
        /// <returns>A value that describes how this command may be handled.</returns>
        public async Task<CommandStatusResult> GetCommandStatusAsync(IImmutableSet<IProjectTree> nodes, long commandId, bool focused, string commandText, CommandStatus progressiveStatus)
        {
            if (commandId == 0x100)
            {
                return new CommandStatusResult(true, commandText, CommandStatus.Enabled | CommandStatus.Supported);
            }
            else
            {
                return CommandStatusResult.Unhandled;
            }
        }

        /// <summary>
        /// Indicates that the user wants to execute a specific command.
        /// </summary>
        /// <param name="nodes">The project nodes to execute on.</param>
        /// <param name="commandId">The command ID.</param>
        /// <param name="focused">A value indicating whether <paramref name="nodes"/> or the project have the user focus.
        ///     A value of <c>false</c> indicates this command is being routed through the application in search of command handlers to process a command that the focused UI did not handle.</param>
        /// <param name="commandExecuteOptions">Values describe how the object should execute the command.</param>
        /// <param name="variantArgIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="variantArgOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <returns>true if the extension has handled execution for this command and should prevent other handlers from processing the command. false otherwise.</returns>
        public async Task<bool> TryHandleCommandAsync(IImmutableSet<IProjectTree> nodes, long commandId, bool focused, long commandExecuteOptions, IntPtr variantArgIn, IntPtr variantArgOut)
        {
            await threadHandler.SwitchToUIThread();
            var result = false;
            var windowFrame = await FindOrCreateWindowFrameForProjectAsync();
            windowFrame.Show();
            return result;
        }


        // Window creation code below adapted from the NuGet project, licensed to the .NET Foundation

        private async Task<IVsWindowFrame> FindOrCreateWindowFrameForProjectAsync()
        {
            var frame = FindExistingWindowFrame();
            if (frame == null)
            {
                frame = await CreateWindowFrameAsync();
            }
            return frame;
        }

        private async Task<IVsWindowFrame> CreateWindowFrameAsync()
        {
            var documentName = project.FullPath;
            var docData = IntPtr.Zero;
            var hr = 0;
            uint itemId;
            IVsHierarchy hierarchy;
            try
            {
                uint cookie;
                hr = docTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock,
                        documentName, out hierarchy, out itemId, out docData, out cookie);
                if (hr != VSConstants.S_OK)
                {
                    //No registered document window.
                    hierarchy = ProjectHierarchy;
                    itemId = (uint)VSConstants.VSITEMID.Root;
                }
            }
            finally
            {
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                    docData = IntPtr.Zero;
                }
            }
            return await CreateDocWindowAsync(project, documentName, hierarchy, itemId);
        }

        private async Task<IVsWindowFrame> CreateDocWindowAsync(UnconfiguredProject unconfiguredProject, string documentName, IVsHierarchy hierarchy, uint itemId)
        {
            var windowFlags = _VSRDTFLAGS.RDT_DontAddToMRU | _VSRDTFLAGS.RDT_DontAutoOpen;
            var model = new WindowModel(project, targetUIs, lockService, threadHandler);
            await model.LoadFromProject();
            var control = new Control(model);
            var windowPane = new WindowPane(control);
            var editorType = Guid.Empty;
            var commandUI = Guid.Empty;
            var caption = $"{model.Name} Deploy Rules";
            IVsWindowFrame frame;
            var docView = IntPtr.Zero;
            var docData = IntPtr.Zero;
            var hr = 0;

            try
            {
                docView = Marshal.GetIUnknownForObject(windowPane);
                docData = Marshal.GetIUnknownForObject(model);
                hr = uiShell.CreateDocumentWindow((uint)windowFlags, documentName, (IVsUIHierarchy)hierarchy, itemId,
                    docView, docData, ref editorType, null, ref commandUI, null, caption, string.Empty, null, out frame);
            }
            finally
            {
                if (docView != IntPtr.Zero)
                {
                    Marshal.Release(docView);
                }
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                }
            }
            ErrorHandler.ThrowOnFailure(hr);
            return frame;
        }

        private IVsWindowFrame FindExistingWindowFrame()
        {
            foreach (var frame in VsUtility.GetDocumentWindows(uiShell))
            {
                object docView;
                var hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView);
                if (hr == VSConstants.S_OK && docView is WindowPane)
                {
                    var pane = docView as WindowPane;
                    var model = (pane.Content as Control).Model;
                    if (model.Name == System.IO.Path.GetFileNameWithoutExtension(project.FullPath))
                    {
                        return frame;
                    }
                }
            }
            return null;
        }
    }
}
