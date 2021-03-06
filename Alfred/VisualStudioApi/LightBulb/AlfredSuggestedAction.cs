﻿using Ask_Alfred.Infrastructure.Interfaces;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
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
        private const string m_display = "Ask Alfred";
        private readonly SuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;
        public AlfredSuggestedAction(SuggestedActionsSourceProvider i_SuggestedActionsSourceProvider, ITextView i_TextView, ITextBuffer i_TextBuffer)
        {
            m_factory = i_SuggestedActionsSourceProvider;
            m_textBuffer = i_TextBuffer;
            m_textView = i_TextView;
        }

        public string DisplayText
        {
            get { return m_display; }
        }
        public ImageMoniker IconMoniker
        {
            get
            {
                return default(ImageMoniker);
            }
        }
        public bool HasActionSets
        {
            get
            {
                return true;
            }
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

        [Obsolete]
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            AlfredSuggestedActionItem actionItem;
            List<SuggestedActionSet> suggestedActionSetList = new List<SuggestedActionSet>();
            List<VisualStudioErrorCodeItem> visualStudioErrorCodeItemList = VisualStudioHandler.GetCurrentLineErrorList();

            foreach (VisualStudioErrorCodeItem codeItem in visualStudioErrorCodeItemList)
            {
                actionItem = new AlfredSuggestedActionItem(codeItem);
                suggestedActionSetList.Add(new SuggestedActionSet(new ISuggestedAction[] { actionItem }));
            }

            return System.Threading.Tasks.Task.FromResult<IEnumerable<SuggestedActionSet>>(suggestedActionSetList);
        }
        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }
        public void Invoke(CancellationToken cancellationToken)
        {
        }
        private IVsWindowFrame openAlfredWithIVsUIShell()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsUIShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            Guid guid = typeof(AlfredWindow).GUID;
            IVsWindowFrame windowFrame;

            int result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out windowFrame);   // Find MyToolWindow

            if (result != VSConstants.S_OK)
                result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Create MyToolWindow if not found

            if (result == VSConstants.S_OK)                                                                           // Show MyToolWindow
                ErrorHandler.ThrowOnFailure(windowFrame.Show());

            return windowFrame;
        }
        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample action and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
