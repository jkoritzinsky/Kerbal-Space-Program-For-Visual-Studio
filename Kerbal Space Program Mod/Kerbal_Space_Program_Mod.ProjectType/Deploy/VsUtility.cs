using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy
{
    // This class is adapted from the NuGet project as licensed to the
    // .NET Foundation

    internal class VsUtility
    {
        public static IEnumerable<IVsWindowFrame> GetDocumentWindows(IVsUIShell uiShell)
        {
            IEnumWindowFrames documentWindowEnumerator;
            var hr = uiShell.GetDocumentWindowEnum(out documentWindowEnumerator);
            if (documentWindowEnumerator == null)
            {
                yield break;
            }

            IVsWindowFrame[] windowFrames = new IVsWindowFrame[1];
            uint frameCount;
            while (documentWindowEnumerator.Next(1, windowFrames, out frameCount) == VSConstants.S_OK &&
                   frameCount == 1)
            {
                yield return windowFrames[0];
            }
        }

        // Gets the package manager control hosted in the window frame.
        public static Control GetDeployConfigurationControl(IVsWindowFrame windowFrame)
        {
            object property;
            var hr = windowFrame.GetProperty(
                (int)__VSFPROPID.VSFPROPID_DocView,
                out property);

            var windowPane = property as WindowPane;
            if (windowPane == null)
            {
                return null;
            }

            var packageManagerControl = windowPane.Content as Control;
            return packageManagerControl;
        }
    }

}
