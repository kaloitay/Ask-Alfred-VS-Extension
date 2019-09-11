using Ask_Alfred.Infrastructure.Interfaces;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Ask_Alfred.UI.VisualStudioApi.LightBulbTest
{
    class AlfredSuggestedActionItem : ISuggestedAction
    {
        private AlfredInput m_Input;

        public bool HasActionSets
        {
            get { return false; }
        }
        public string DisplayText
        {
            get { return m_Input.Description; }
        }
        public ImageMoniker IconMoniker
        {
            // TODO:
            get { return default(ImageMoniker); }
        }
        public string IconAutomationText
        {
            get
            {
                return null;
            }
        }
        public string InputGestureText
        {
            get
            {
                return null;
            }
        }
        public bool HasPreview
        {
            get { return false; }
        }
        public void Dispose()
        {
        }

        public AlfredSuggestedActionItem(VisualStudioErrorCodeItem i_Errror)
        {
            m_Input = new AlfredInput(i_Errror.Description, i_Errror.ErrorCode, i_Errror.Project);
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsWindowFrame windowFrame = openAlfredWithIVsUIShell();

            AskAlfredWindow alfred = getAlfredToolWindow(windowFrame);
            alfred.SearchSpecificInput(m_Input);
        }

        private IVsWindowFrame openAlfredWithIVsUIShell()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsUIShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            Guid guid = typeof(AskAlfredWindow).GUID;
            IVsWindowFrame windowFrame;

            int result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out windowFrame);   // Find MyToolWindow

            if (result != VSConstants.S_OK)
                result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Create MyToolWindow if not found

            if (result == VSConstants.S_OK)                                                                           // Show MyToolWindow
                ErrorHandler.ThrowOnFailure(windowFrame.Show());

            return windowFrame;
        }

        private AskAlfredWindow getAlfredToolWindow(IVsWindowFrame i_WindowFrame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            i_WindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object window);

            if (window is AskAlfredWindow)
                return window as AskAlfredWindow;

            throw new NotSupportedException("Cannot get Alfred tool window.");
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample action and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
