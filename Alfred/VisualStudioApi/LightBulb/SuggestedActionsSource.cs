using Ask_Alfred.UI.VisualStudioApi.LightBulbTest;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ask_Alfred.UI.VisualStudioApi
{
    internal class SuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly SuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public SuggestedActionsSource(SuggestedActionsSourceProvider suggestedActionsSourceProvider, ITextView textView, ITextBuffer textBuffer)
        {
            m_factory = suggestedActionsSourceProvider;
            m_textBuffer = textBuffer;
            m_textView = textView;
        }

        public async Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return true;
        }

        [Obsolete]
        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            AlfredSuggestedAction alfredAction = null;

            if (VisualStudioHandler.GetCurrentLineErrorList().Count > 0)
            {
                alfredAction = new AlfredSuggestedAction(m_factory, m_textView, m_textBuffer);
                return new SuggestedActionSet[] {
                    new SuggestedActionSet(new ISuggestedAction[] { alfredAction })
                };
            }
            else
                return null;
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }

        private bool tryGetWordUnderCaret(out TextExtent o_WordExtent)
        {
            ITextCaret caret = m_textView.Caret;
            SnapshotPoint point = caret.Position.BufferPosition;

            if (caret.Position.BufferPosition <= 0)
            {
                o_WordExtent = default(TextExtent);
                return false;
            }

            ITextStructureNavigator navigator = m_factory.NavigatorService.GetTextStructureNavigator(m_textBuffer);

            o_WordExtent = navigator.GetExtentOfWord(point);
            return true;
        }
    }
}
