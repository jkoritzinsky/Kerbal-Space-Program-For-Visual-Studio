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

namespace KSP4VS.Deploy
{
    /// <summary>
    /// Provides implementation for handling commands.
    /// </summary>
    // TODO: If your implementation is async, consider using IAsyncCommandGroupHandler instead of ICommandGroupHandler
    [ExportCommandGroup("f61a30ca-328c-4a2c-b394-f5f4a5eefcd4")]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    internal class DeployCommandGroupHandler : IAsyncCommandGroupHandler
    {
        private UnconfiguredProject project;
        private IThreadHandling threadHandler;

        private IVsUIShell uiShell;
        private IVsRunningDocumentTable docTable;

        [ImportingConstructor]
        private DeployCommandGroupHandler(UnconfiguredProject project, IThreadHandling threadHandler, [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.threadHandler = threadHandler;
            threadHandler.VerifyOnUIThread();
            this.project = project;
            uiShell = (IVsUIShell)serviceProvider.GetService(typeof(IVsUIShell));
            docTable = (IVsRunningDocumentTable)serviceProvider.GetService(typeof(IVsRunningDocumentTable));
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
            var windowFrame = await FindOrCreateWindowFrameAsync(project);
            windowFrame.Show();
            return result;
        }

        private async Task<IVsWindowFrame> FindOrCreateWindowFrameAsync(UnconfiguredProject project)
        {
            var frame = FindExistingWindowFrame(project);
            if (frame == null)
            {
                frame = await CreateWindowFrameAsync(project);
            }
            return frame;
        }

        private Task<IVsWindowFrame> CreateWindowFrameAsync(UnconfiguredProject project)
        {
            throw new NotImplementedException();
        }

        private IVsWindowFrame FindExistingWindowFrame(UnconfiguredProject project)
        {
            throw new NotImplementedException();
        }
    }
}
