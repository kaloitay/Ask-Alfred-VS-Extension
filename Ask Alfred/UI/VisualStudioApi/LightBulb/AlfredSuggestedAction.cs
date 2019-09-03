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
    class AlfredSuggestedAction : ISuggestedAction
    {
        //private ITrackingSpan m_span;
        //private string m_upper; // *** instead of m_upper - alfred first result title
        private string m_error;
        private string m_display;
        //private ITextSnapshot m_snapshot;

        public bool HasActionSets
        {
            get { return false; }
        }
        public string DisplayText
        {
            get { return m_display; }
        }
        public ImageMoniker IconMoniker
        {
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
            get { return true; }
        }
        public void Dispose()
        {
        }

        //public AlfredSuggestedAction(ITrackingSpan span)
        public AlfredSuggestedAction(IAlfredInput i_Input)
        {
            // m_span = span;
            // m_snapshot = span.TextBuffer.CurrentSnapshot;
            // m_upper = span.GetText(m_snapshot).ToUpper();

            m_error = i_Input.Description;//i_ErrorDescription;//VisualStudioHandler.GetCurrentLineErrorDescription();

            m_display = string.Format("Ask Alfred: '{0}'", m_error);
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = "Preview of response" }); // *** insert here a first result title ?
            return System.Threading.Tasks.Task.FromResult<object>(textBlock);
        }

        public void Invoke(CancellationToken cancellationToken)
        { // *** here we will present alfreds window
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsWindowFrame windowFrame = openAlfredWithIVsUIShell();

            AskAlfredWindow alfred = getAlfredToolWindow(windowFrame);

            // TODO: GAL help fix that error in the line below!!!!!!!!!!!!!!!!!
            // alfred.AutoSearchByText(m_error);
        }

        private IVsWindowFrame openAlfredWithIVsUIShell()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsUIShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
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
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

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
