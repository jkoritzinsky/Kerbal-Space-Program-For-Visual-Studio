using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace KSP4VS.Deploy
{
    public class WindowPane : Microsoft.VisualStudio.Shell.WindowPane, IVsWindowFrameNotify3
    {
        private Control content;
        public WindowPane(Control deployControl)
            :base(null)
        {
            content = deployControl;
        }

        public override object Content => content;

        public int OnShow(int fShow)
        {
            return VSConstants.S_OK;
            throw new NotImplementedException();
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
            content.Model.Save();
            pgrfSaveOptions = (uint)__FRAMECLOSE.FRAMECLOSE_NoSave;
            return VSConstants.S_OK;
        }
    }
}