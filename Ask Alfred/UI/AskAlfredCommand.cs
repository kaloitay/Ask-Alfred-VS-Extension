using Ask_Alfred.UI.VisualStudioApi;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Ask_Alfred.UI
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AskAlfredCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int toolsdId = 4436;
        public const int errorListId = 512;
        public const int textEditorId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid toolsGuid = new Guid("d4677dee-3ed3-4fa4-8483-2eea77c80219");
        public static readonly Guid errorListGuid = new Guid("9d9046da-94f8-4fd0-8a00-92bf4f6defa8");
        public static readonly Guid textEditorGuid = new Guid("aa0a3a2d-7952-4b57-89f1-73981024d2a7");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AskAlfredCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AskAlfredCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            // tools menu
            CommandID toolsMenuCommandID = new CommandID(toolsGuid, toolsdId);
            MenuCommand toolsMenuItem = new MenuCommand(this.ExecuteToolMenu, toolsMenuCommandID);
            commandService.AddCommand(toolsMenuItem);

            // text editor menu
            CommandID textEditorMenuCommandID = new CommandID(textEditorGuid, textEditorId);
            OleMenuCommand textEditorMenuItem = new OleMenuCommand(this.ExecuteToolMenuTextEditorMenu, textEditorMenuCommandID);
            textEditorMenuItem.BeforeQueryStatus += new EventHandler(this.textEditorMenuItem_BeforeQueryStatus);
            commandService.AddCommand(textEditorMenuItem);

            // error list menu
            CommandID errorListCommandID = new CommandID(errorListGuid, errorListId);
            OleMenuCommand errorMenuItem = new OleMenuCommand(this.ExecuteErrrorListMenu, errorListCommandID);
            errorMenuItem.BeforeQueryStatus += new EventHandler(this.errorMenuItem_BeforeQueryStatus);
            commandService.AddCommand(errorMenuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AskAlfredCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ErrorsToolWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new AskAlfredCommand(package, commandService);
        }
        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ExecuteToolMenu(object sender, EventArgs e)
        {
            // *** CODE DUPLICATION extracted to method but havent used here
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.package.FindToolWindow(typeof(AskAlfredWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            AskAlfredWindow askAlfredWindow = window as AskAlfredWindow;
            //askAlfredWindow.SearchSelectedErrorAsync();

            // if there is selected error so search the error
        }

        private void ExecuteToolMenuTextEditorMenu(object sender, System.EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            ToolWindowPane window = this.package.FindToolWindow(typeof(AskAlfredWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            AskAlfredWindow askAlfredWindow = window as AskAlfredWindow;
            string selectedText = VisualStudioHandler.GetCurrentLineSelectedText();
            AlfredInput alfredInput = AlfredInputManager.Instance.GetInputForAlfredWindowSearchBar(selectedText);
            askAlfredWindow.SearchSpecificInput(alfredInput);
        }

        private void errorMenuItem_BeforeQueryStatus(object sender, System.EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            ToolWindowPane window = this.package.FindToolWindow(typeof(AskAlfredWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            AskAlfredWindow askAlfredWindow = window as AskAlfredWindow;
            if (askAlfredWindow != null && VisualStudioHandler.HasSelectedError())
            {
                menuItem.Enabled = true;
                menuItem.Visible = true;
            }
            else
            {
                menuItem.Enabled = false;
                menuItem.Visible = false;
            }
        }

        private void textEditorMenuItem_BeforeQueryStatus(object sender, System.EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            ToolWindowPane window = this.package.FindToolWindow(typeof(AskAlfredWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            AskAlfredWindow askAlfredWindow = window as AskAlfredWindow;
            if (askAlfredWindow != null && !String.IsNullOrEmpty(VisualStudioHandler.GetCurrentLineSelectedText()))
            {
                menuItem.Enabled = true;
                menuItem.Visible = true;
            }
            else
            {
                menuItem.Enabled = false;
                menuItem.Visible = false;
            }
        }

        private void ExecuteErrrorListMenu(object sender, EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            ToolWindowPane window = this.package.FindToolWindow(typeof(AskAlfredWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            AskAlfredWindow askAlfredWindow = window as AskAlfredWindow;
            askAlfredWindow.SearchSelectedErrorAsync();
        }
    }
}