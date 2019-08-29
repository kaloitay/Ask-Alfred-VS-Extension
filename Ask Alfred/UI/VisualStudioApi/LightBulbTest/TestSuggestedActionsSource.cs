using Ask_Alfred.UI.VisualStudioApi.LightBulbTest;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ask_Alfred.UI.VisualStudioApi
{
    internal class TestSuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly TestSuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;

        public TestSuggestedActionsSource(TestSuggestedActionsSourceProvider testSuggestedActionsSourceProvider, ITextView textView, ITextBuffer textBuffer)
        {
            m_factory = testSuggestedActionsSourceProvider;
            m_textBuffer = textBuffer;
            m_textView = textView;
        }

        private bool TryGetWordUnderCaret(out TextExtent wordExtent)
        {
            ITextCaret caret = m_textView.Caret;
            SnapshotPoint point;

            if (caret.Position.BufferPosition > 0)
            {
                point = caret.Position.BufferPosition - 1;
            }
            else
            {
                wordExtent = default(TextExtent);
                return false;
            }

            ITextStructureNavigator navigator = m_factory.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            
            wordExtent = navigator.GetExtentOfWord(point);

            return true;
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            { // *** here i will check if there are errors / selected text / what ever triggers alfred
                //TextExtent extent;
                //if (TryGetWordUnderCaret(out extent))
                //{
                //    // don't display the action if the extent has whitespace
                //    return extent.IsSignificant;
                //}
                return VisualStudioHandler.IsCurrentLineHasError();
                // return false;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            TextExtent extent;
            if (TryGetWordUnderCaret(out extent) && extent.IsSignificant)
            {
                ITrackingSpan trackingSpan = range.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
             //   var upperAction = new UpperCaseSuggestedAction(trackingSpan);

                var alfredAction = new AlfredSuggestedAction(VisualStudioHandler.GetCurrentLineErrorDescription()); // *** not sure that tracking span required for me
                
                return new SuggestedActionSet[] { new SuggestedActionSet(new ISuggestedAction[] { alfredAction }) };
            }
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
