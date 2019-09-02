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
    public class AskAlfredWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AskAlfredWindow"/> class.
        /// </summary>
        public AskAlfredWindow() : base(null)
        {
            this.Caption = "Ask Alfred";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new AskAlfredWindowControl();
        }

        internal void AutoSearch()
        {
            (Content as AskAlfredWindowControl).AutoSearch();
        }
    }
}
