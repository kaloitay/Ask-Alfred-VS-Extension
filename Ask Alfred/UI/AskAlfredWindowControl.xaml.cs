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
    using System.Net;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        private AlfredInputManager m_AlfredInputManager;
        private ArrayList m_SortedRankArray = new ArrayList();
        private ComboBoxViewModel m_HistorySearchesViewModel = new ComboBoxViewModel();
        private const string k_NoInternetString = "No internet connection";
        private const string k_SlowInternetString = "Slow internet connection. Partial results presented";
        private const string k_UnexpectedErrorString = "Unexpected error";

        public AskAlfredWindowControl()
        {
            InitializeComponent();
            initializeAskAlfredWindow();
        }

        private void initializeAskAlfredWindow()
        {
            m_AlfredInputManager = new AlfredInputManager();
            resultsListView.Items.Clear();
            searchComboBox.Text = string.Empty;
            searchingForTextBlock.Text = string.Empty;
            DataContext = m_HistorySearchesViewModel;
            searchingImage.Visibility = Visibility.Hidden;
            notSearchingImage.Visibility = Visibility.Hidden;
            AlfredEngine.Instance.OnPageAdded += pageAddedHandler;
        }

        private void pageAddedHandler(IPage i_Page)
        {
            createResultItem(i_Page);
        }

        private void stopSearchAndInsertMessage(string i_Text)
        {
            searchIsFinished();

            Application.Current.Dispatcher.Invoke(() =>
            {
                resultsListView.Items.Insert(m_SortedRankArray.Count, new TextBlock
                {
                    Text = i_Text,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
            });
        }

        private void noInternetConnection()
        {
            stopSearchAndInsertMessage(k_NoInternetString);
        }

        private void timeoutExpired()
        {
            stopSearchAndInsertMessage(k_SlowInternetString);
        }

        private void unexpectedError()
        {
            stopSearchAndInsertMessage(k_UnexpectedErrorString);
        }

        private void searchIsFinished()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                searchingForTextBlock.Text = "Found " + resultsListView.Items.Count + " results";
                searchingImage.IsEnabled = false;
                searchingImage.Visibility = Visibility.Hidden;
                notSearchingImage.Visibility = Visibility.Visible;
            });
        }

        private void createResultItem(IPage i_Page)
        {
            if (i_Page != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AskAlfredResultUIElement askAlfredResultUIElement = new AskAlfredResultUIElement(i_Page, this.Resources);

                    int resultIndex = insertPageToSortedRankArray(i_Page.Rank);
                    resultsListView.Items.Insert(resultIndex, askAlfredResultUIElement.dockPanel);
                });
            }
        }

        public async System.Threading.Tasks.Task SearchSpecificInputAsync(IAlfredInput i_Input)
        {
            await SearchAsync(i_Input);
        }

        private int insertPageToSortedRankArray(double i_Rank)
        {
            int resultIndex;

            for (resultIndex = 0; resultIndex < m_SortedRankArray.Count; ++resultIndex)
            {
                if ((double)m_SortedRankArray[resultIndex] < i_Rank)
                    break;
            }

            m_SortedRankArray.Insert(resultIndex, i_Rank);
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

            if (!m_HistorySearchesViewModel.HistorySearches.Contains(searchComboBox.Text))
                m_HistorySearchesViewModel.HistorySearches.Insert(0, searchComboBox.Text);

            try
            {
                await AlfredEngine.Instance.SearchAsync(i_Input);
                searchIsFinished();
            }
            catch (WebException)
            {
                noInternetConnection();
            }
            catch (OperationCanceledException)
            {
                timeoutExpired();
            }
            catch (Exception)
            {
                unexpectedError();
            }
        }

        private void setAskAlfredWindowForNewSearch(IAlfredInput i_Input)
        {
            searchingImage.IsEnabled = true;
            searchingImage.Visibility = Visibility.Visible;
            notSearchingImage.Visibility = Visibility.Hidden;
            resultsListView.Items.Clear();
            searchComboBox.Text = i_Input.Description;
            searchingForTextBlock.Text = "Searching...";

            m_SortedRankArray.RemoveRange(0, m_SortedRankArray.Count);
        }

        private void searchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!String.IsNullOrEmpty(searchComboBox.Text) && e.Key == Key.Enter && searchComboBox.IsEnabled == true)
            {
                AlfredInput alfredInput = m_AlfredInputManager.GetInputForAlfredWindowSearchBar(searchComboBox.Text);
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
    }
}