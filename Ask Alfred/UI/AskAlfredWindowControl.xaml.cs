namespace Ask_Alfred.UI
{
    using Ask_Alfred.Infrastructure;
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using EnvDTE;
    using Package = Microsoft.VisualStudio.Shell.Package;
    using Ask_Alfred.Infrastructure.Interfaces;
    using System.Windows.Input;
    using System;
    using Ask_Alfred.UI.VisualStudioApi;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        private AlfredInputManager m_AlfredInputManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AskAlfredWindowControl"/> class.
        /// </summary>
        public AskAlfredWindowControl()
        {
            InitializeComponent();
            initializeAskAlfredWindow();
        }

        private void initializeAskAlfredWindow()
        {
            m_AlfredInputManager = new AlfredInputManager();
            resultsListView.Items.Clear();
            AlfredEngine.Instance.OnPageAdded += pageAddedHandler;
            AlfredEngine.Instance.OnTimeoutExpired += timeoutExpiredHandler;
        }

        private void pageAddedHandler(IPage i_Page)
        {
            if (i_Page != null)
            {
                // use stackoverflowPage.Rank to get the rank
                createWindowResult(i_Page);
            }
            // else
        }
        private void searchIsFinished()
        {
            searchComboBox.IsEnabled = true;
        }
        private void timeoutExpiredHandler()
        {
            //searchComboBox.IsEnabled = true;
        }
        private void createWindowResult(IPage i_Page)
        {
            AskAlfredResultUIElement askAlfredResultUIElement = new AskAlfredResultUIElement(i_Page, this.Resources);
            resultsListView.Items.Add(askAlfredResultUIElement.dockPanel);
        }

        private void askAlfredSearch(IAlfredInput i_Input)
        {
            AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfred();

            setAskAlfredWindowForNewSearch(i_Input);
            searchByInputAsync(i_Input);
        }

        private void setAskAlfredWindowForNewSearch(IAlfredInput i_Input)
        {
            searchComboBox.IsEnabled = false;
            resultsListView.Items.Clear();
            searchComboBox.Text = i_Input.Description;
            searchingForTextBlock.Text = "Searching For '" + i_Input.Description + "'";
        }

        private async System.Threading.Tasks.Task searchByInputAsync(IAlfredInput i_Input)
        {
            AlfredResponse response = await AlfredEngine.Instance.SearchAsync(i_Input);
            searchIsFinished();
        }

        private void searchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && searchComboBox.IsEnabled == true)
            {
                AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfredWindowSearchBar(searchComboBox.Text);
                askAlfredSearch(alfredInput);
            }
        }

        public void AutoSearch()
        {
            AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfred();
            askAlfredSearch(alfredInput);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView)
            {
                DockPanel selectedDockPanel = (DockPanel)(sender as ListView).SelectedItems[0];

                if (selectedDockPanel != null)
                {
                    System.Diagnostics.Process.Start(selectedDockPanel.Tag.ToString());
                }
            }
        }
    }
}