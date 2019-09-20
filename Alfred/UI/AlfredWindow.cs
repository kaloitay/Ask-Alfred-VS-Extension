namespace Ask_Alfred.UI
{
    using Ask_Alfred.Infrastructure.Interfaces;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("712047d6-ae52-4fa1-87b9-9e9e5669e5bc")]
    public class AlfredWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlfredWindow"/> class.
        /// </summary>
        public AlfredWindow() : base(null)
        {
            this.Caption = "Alfred";
            this.Content = new AlfredWindowControl();
        }

        internal async System.Threading.Tasks.Task SearchSelectedErrorAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AlfredWindowControl content = Content as AlfredWindowControl;
            await content.SearchSelectedError();
        }
        internal async System.Threading.Tasks.Task SearchSpecificInput(IAlfredInput i_Input)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AlfredWindowControl content = Content as AlfredWindowControl;
            await content.SearchSpecificInputAsync(i_Input);
        }
    }
}
