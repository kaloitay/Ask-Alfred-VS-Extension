namespace Ask_Alfred.UI
{
    using Ask_Alfred.Infrastructure;
    using Microsoft.VisualStudio.Shell;
    using System.Windows;
    using System.Windows.Controls;
    using Ask_Alfred.Infrastructure.Interfaces;
    using System.Windows.Input;
    using Ask_Alfred.UI.VisualStudioApi;
    using System.Collections;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
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
            resultsListView.Items.Clear();
            searchComboBox.Text = string.Empty;
            searchingForTextBlock.Text = string.Empty;
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
            searchingImage.IsEnabled = false;
            searchingImage.Visibility = Visibility.Hidden;
        }
        private void timeoutExpiredHandler()
        {
            //searchComboBox.IsEnabled = true;
        }
        private void createWindowResult(IPage i_Page)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AskAlfredResultUIElement askAlfredResultUIElement = new AskAlfredResultUIElement(i_Page, this.Resources);

                int resultIndex = insertPageToSortedRankArray(i_Page.Rank);
                resultsListView.Items.Insert(resultIndex, askAlfredResultUIElement.dockPanel);
            });
        }

        public async System.Threading.Tasks.Task SearchSpecificInputAsync(IAlfredInput i_Input)
        {
            await SearchAsync(i_Input);
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

        public async System.Threading.Tasks.Task SearchSelectedError()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AlfredInput alfredInput = AlfredInputManager.Instance.GetInputFromSelectedError();
            await SearchAsync(alfredInput);
        }

        private async System.Threading.Tasks.Task SearchAsync(IAlfredInput i_Input)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            setAskAlfredWindowForNewSearch(i_Input);
            await AlfredEngine.Instance.SearchAsync(i_Input);
            searchIsFinished();
        }

        private void setAskAlfredWindowForNewSearch(IAlfredInput i_Input)
        {
            searchComboBox.IsEnabled = false;
            searchingImage.IsEnabled = true;
            searchingImage.Visibility = Visibility.Visible;
            resultsListView.Items.Clear();
            searchComboBox.Text = i_Input.Description;
            searchingForTextBlock.Text = "Searching For '" + i_Input.Description + "'";
            sortedRankArray.RemoveRange(0, sortedRankArray.Count);
        }

        private void searchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (e.Key == Key.Enter && searchComboBox.IsEnabled == true)
            {
                AlfredInput alfredInput = AlfredInputManager.Instance.GetInputForAlfredWindowSearchBar(searchComboBox.Text);
                SearchAsync(alfredInput);
            }
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

        private void StopSearchButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //AlfredEngine.Instance.StopSearch();
        }
    }
}