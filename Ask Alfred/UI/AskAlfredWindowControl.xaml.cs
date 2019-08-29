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
    using Ask_Alfred.Objects;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        private readonly double RESULT_HEIGHT = 55;
        private readonly double REUSLT_DATE_FONT_SIZE = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="AskAlfredWindowControl"/> class.
        /// </summary>
        public AskAlfredWindowControl()
        {
            this.InitializeComponent();
            IntializeAskAlfredWindow();
        }

        private void IntializeAskAlfredWindow()
        {
            AlfredEngine.Instance.OnPageAdded += pageAddedHandler;
            AlfredEngine.Instance.OnTimeoutExpired += timeoutExpiredHandler;

            mainResultsStackPanel.Children.Clear();
        }

        private void pageAddedHandler(IPage page)
        {
            if (page != null)
            {
                // use stackoverflowPage.Rank to get the rank
                createWindowResult(page);
            }
            // else...
        }

        private void createWindowResult(IPage page)
        {
            StackPanel resultStackPanel = new StackPanel();
            resultStackPanel.MouseEnter += ResultStackPanel_MouseEnter;
            resultStackPanel.MouseLeave += ResultStackPanel_MouseLeave;
            //resultStackPanel.MouseDown += ResultStackPanel_MouseDown(page);
            resultStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            resultStackPanel.Orientation = Orientation.Vertical;


            DockPanel resultDockPanel = new DockPanel();
            resultDockPanel.VerticalAlignment = VerticalAlignment.Top;
            resultDockPanel.Height = RESULT_HEIGHT;

            Image resultImage = new Image();
            DockPanel.SetDock(resultImage, Dock.Left);
            resultImage.Height = 10;
            resultImage.Width = 10;
            //resultImage.Source = new BitmapImage(new Uri(@"\Resources\Icons\go_to_web_icon.png", UriKind.Relative));

            TextBlock resultDateTextBlock = new TextBlock();
            DockPanel.SetDock(resultDateTextBlock, Dock.Top);
            resultDateTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
            resultDateTextBlock.FontSize = REUSLT_DATE_FONT_SIZE;
            resultDateTextBlock.Foreground = new SolidColorBrush(Colors.White);
            resultDateTextBlock.Text = page.Date.ToString();

            StackPanel websiteStackPanel = new StackPanel();
            DockPanel.SetDock(websiteStackPanel, Dock.Bottom);
            websiteStackPanel.Orientation = Orientation.Horizontal;
            websiteStackPanel.HorizontalAlignment = HorizontalAlignment.Right;

            if (page is StackoverflowPage)
            {
                StackoverflowPage stackoverflowPage = page as StackoverflowPage;

                if (stackoverflowPage.IsAnswered) // TODO: need to fix this condition to green v condition
                {
                    Image greenVImage = new Image();
                    //greenVImage.Source = new BitmapImage(new Uri(@"\Resources\Icons\green_checkmark_stackoverflow_icon.png", UriKind.Relative));
                    greenVImage.Height = 7;
                    greenVImage.Width = 7;
                    websiteStackPanel.Children.Add(greenVImage);
                }
            }

            TextBlock websiteNameTextBlock = new TextBlock();
            websiteNameTextBlock.Text = page.WebsiteName;
            websiteNameTextBlock.FontSize = 7;
            websiteNameTextBlock.Foreground = new SolidColorBrush(Colors.White);
            websiteStackPanel.Children.Add(websiteNameTextBlock);

            TextBlock resultSubjectTextBlock = new TextBlock();
            resultDateTextBlock.Text = page.Subject;
            resultDateTextBlock.TextWrapping = TextWrapping.Wrap;
            resultDateTextBlock.FontSize = 9;
            resultDateTextBlock.Foreground = new SolidColorBrush(Colors.White);
            resultDateTextBlock.TextAlignment = TextAlignment.Justify;
            resultDateTextBlock.VerticalAlignment = VerticalAlignment.Center;

            Separator separator = new Separator();
            separator.HorizontalAlignment = HorizontalAlignment.Center;
            separator.Width = 343;

            resultDockPanel.Children.Add(resultImage);
            resultDockPanel.Children.Add(resultDateTextBlock);
            resultDockPanel.Children.Add(websiteStackPanel);
            resultDockPanel.Children.Add(resultSubjectTextBlock);

            resultStackPanel.Children.Add(resultDockPanel);
            resultStackPanel.Children.Add(separator);
            mainResultsStackPanel.Children.Add(resultStackPanel);
        }

        private void ResultStackPanel_MouseDown(object sender, MouseButtonEventArgs e, IPage page)
        {
            System.Diagnostics.Process.Start(page.Url);
        }
        // TODO: fix to happen only once
        private void ResultStackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel currentStackPanel = sender as StackPanel;

            if (currentStackPanel.IsMouseOver)
            {
                currentStackPanel.Background = null;
            }
        }
        // TODO: fix to happen only once
        private void ResultStackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel currentStackPanel = sender as StackPanel;

            if (!currentStackPanel.IsMouseOver)
            {
                currentStackPanel.Background = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
            }
        }

        private void searchIsFinished()
        {
            //searchComboBox.IsEnabled = true;
    /*        System.Windows.MessageBox.Show("Search is finished",
                "Search is finished with " + dataGridViewPages.Items.Count + " results",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);*/
        }
        private void timeoutExpiredHandler()
        {
            //searchComboBox.IsEnabled = true;
            /*  System.Windows.MessageBox.Show("Timeout Expired",
                  "Timeout Expired after " + dataGridViewPages.Items.Count + " results",
                  MessageBoxButton.OK, MessageBoxImage.Exclamation);*/
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string errorDescription = getErrorDescription();
            string errorCode = getErrorCode();

            if (errorDescription == null)
            {
                //dataGridViewPages.Items.Add("No errors detected.");
            }
            else
            {
                //dataGridViewPages.Items.Clear();
                SearchBySelectedTextAsync(errorDescription); // TODO: change method name
            }
        }

        public async System.Threading.Tasks.Task SearchBySelectedTextAsync(string i_SelectedText)
        {
            // TODO: find a normal way to do it
            // ESupportedProgrramingLanguages currentLanguage = ESupportedProgrramingLanguages.CSharp;

            AlfredResponse response = await AlfredEngine.Instance.SearchAsync(i_SelectedText);

            searchIsFinished();
            // IPage or IWebDataSource?
            //foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
            //     .Where(mytype => mytype.GetInterfaces().Contains(typeof(IPage))))
            //{
            //    var props = mytype.GetFields();

            //    //Console.WriteLine(props[0].GetValue(null));
            //}
        }

        // getErrorDescription returns the description of the first error code
        private string getErrorDescription()
        { // should i throw exception if no errors ? 
            string description = null;

            var dte = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(DTE));
            var errorList = dte.ToolWindows.ErrorList;

            if (errorList.ErrorItems.Count > 0)
            {
                // TODO: get SELECTED error description.
                description = errorList.ErrorItems.Item(1).Description;
            }

            //for (int i = 1; i <= errorList.ErrorItems.Count; i++)
            //{
            //    // errorItem contains -> Description, FileName, Line, Column, Project
            //    var errorItem = errorList.ErrorItems.Item(i);

            //    description = errorItem.Description;
            //}

            return description;
        }

        // getErrorCode returns the selected or first error code
        private string getErrorCode()
        {
            string errorCode = null;

            var dte = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(DTE));
            var errorList = dte.ToolWindows.ErrorList as IErrorList;
            var selected = errorList.TableControl.SelectedOrFirstEntry;

            //if (errorList.AreErrorsShown)
            //{
            //    errorList.TableControl.SelectAll();
            //}

            if (selected != null)
            {
                object content; // how do i get the url ?

                if (selected.TryGetValue("errorcode", out content))
                {
                    errorCode = (string)content;
                }
            }

            return errorCode;
        }
        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == System.Windows.Input.Key.Enter && searchComboBox.IsEnabled == true)
            {
                searchComboBox.IsEnabled = false;
                SearchBySelectedTextAsync(searchComboBox.Text);
            }
        }
    }
}