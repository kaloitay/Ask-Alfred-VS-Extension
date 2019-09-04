namespace Ask_Alfred.UI
{
    using Ask_Alfred.Infrastructure;
    using Microsoft.VisualStudio.Shell;
    using System.Windows;
    using System.Windows.Controls;
    using Ask_Alfred.Infrastructure.Interfaces;
    using System.Windows.Input;
    using Ask_Alfred.UI.VisualStudioApi;
    using System;
    using System.Collections;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        private AlfredInputManager m_AlfredInputManager;
        ArrayList sortedRankArray = new ArrayList();
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
            //ThreadHelper.ThrowIfNotOnUIThread();
            Application.Current.Dispatcher.Invoke(() =>
            {
                AskAlfredResultUIElement askAlfredResultUIElement = new AskAlfredResultUIElement(i_Page, this.Resources);

                int resultIndex = insertPageToSortedRankArray(i_Page.Rank);
                Debug.Write("@@@@@@" + "Subject Text: " + i_Page.Subject + " Rank: " + i_Page.Rank + "@@@@@");
                resultsListView.Items.Insert(resultIndex, askAlfredResultUIElement.dockPanel);
                //resultsListView.Items.Add(askAlfredResultUIElement.dockPanel);
            });
        }

        private int insertPageToSortedRankArray(double i_Rank)
        {
            int resultIndex;

            for (resultIndex = 0; resultIndex < sortedRankArray.Count; ++resultIndex)
            {
                if ((double)sortedRankArray[resultIndex] < i_Rank)
                    break;
            }

            sortedRankArray.Insert(resultIndex, i_Rank);
            return resultIndex;
        }

        private async System.Threading.Tasks.Task askAlfredSearchAsync(IAlfredInput i_Input)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfred();

            setAskAlfredWindowForNewSearch(i_Input);
            await searchByInputAsync(i_Input);
        }

        private void setAskAlfredWindowForNewSearch(IAlfredInput i_Input)
        {
            searchComboBox.IsEnabled = false;
            resultsListView.Items.Clear();
            searchComboBox.Text = i_Input.Description;
            searchingForTextBlock.Text = "Searching For '" + i_Input.Description + "'";
            sortedRankArray.RemoveRange(0, sortedRankArray.Count);
        }

        private async System.Threading.Tasks.Task searchByInputAsync(IAlfredInput i_Input)
        {
            await AlfredEngine.Instance.SearchAsync(i_Input);
            searchIsFinished();
        }

        private void searchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (e.Key == Key.Enter && searchComboBox.IsEnabled == true)
            {
                AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfredWindowSearchBar(searchComboBox.Text);
                askAlfredSearchAsync(alfredInput);
            }
        }

        public async System.Threading.Tasks.Task AutoSearchAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfred();
            await askAlfredSearchAsync(alfredInput);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView)
            {
                DockPanel selectedDockPanel = ((sender as ListView).SelectedItems[0]) as DockPanel;

                if (selectedDockPanel != null)
                {
                    System.Diagnostics.Process.Start(selectedDockPanel.Tag.ToString());
                }
            }
        }
    }
}