namespace Ask_Alfred.UI
{
    using Ask_Alfred.Infrasructure;
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using EnvDTE;
    using Package = Microsoft.VisualStudio.Shell.Package;
    using Ask_Alfred.Infrasructure.Interfaces;
    using Ask_Alfred.Objects;
    using System.Windows.Input;
    using System;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        AlfredEngine m_Engine;
        private readonly double RESULT_HEIGHT = 55;
        private readonly double RESULT_IMAGE_WIDTH;
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
            m_Engine = new AlfredEngine();
            m_Engine.OnPageAdded += pageAddedHandler;
            m_Engine.OnTimeoutExpired += timeoutExpiredHandler;

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
            StackoverflowPage stackoverflowPage = page as StackoverflowPage;

            StackPanel resultStackPanel = new StackPanel();
            resultStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            resultStackPanel.Orientation = Orientation.Vertical;

            DockPanel resultDockPanel = new DockPanel();
            resultDockPanel.VerticalAlignment = VerticalAlignment.Top;
            resultDockPanel.Height = RESULT_HEIGHT;

            Image resultImage = new Image();
            DockPanel.SetDock(resultImage, Dock.Left);
            resultImage.Height = resultDockPanel.Height;
            resultImage.Width = resultDockPanel.Height;
            resultImage.Source = new BitmapImage(new Uri(@"\Resources\result-icon.png", UriKind.Relative));

            TextBlock resultDateTextBlock = new TextBlock();
            DockPanel.SetDock(resultDateTextBlock, Dock.Top);
            resultDateTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
            resultDateTextBlock.FontSize = REUSLT_DATE_FONT_SIZE;
            resultDateTextBlock.Foreground = new SolidColorBrush(Colors.White);

            resultDateTextBlock.Text = stackoverflowPage.Date.ToString();


            mainResultsStackPanel.Children.Add(new StackPanel());
        }
/*
                        < TextBlock x:Name= "DateTextBlock1" DockPanel.Dock= "Top" HorizontalAlignment= "Right" Text= "23/09/2017" FontSize= "5" Foreground= "White" Margin= "0,3,3,0" />
                        < StackPanel DockPanel.Dock= "Bottom" Orientation= "Horizontal" HorizontalAlignment= "Right" >
                            < Image Height= "7" Width= "7" Margin= "0,0,8,0" />
                            < TextBlock x:Name= "WebsiteNameTextBlock1" Text= "StackOverFlow" FontSize= "7" Foreground= "White" Margin= "0,0,8,0" />
                        </ StackPanel >
                        < TextBlock TextWrapping= "Wrap" Text= "Keep mouse's scrollwheel to work on Scrollview when placed on top of Mapview/MapMarkerPopup and auto_dismiss" Margin= "1,5,30,5" FontSize= "9" Foreground= "White" TextAlignment= "Justify" VerticalAlignment= "Center" />
                    </ DockPanel >*/

        private void searchIsFinished()
        {
    /*        System.Windows.MessageBox.Show("Search is finished",
                "Search is finished with " + dataGridViewPages.Items.Count + " results",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);*/
        }
        private void timeoutExpiredHandler()
        {
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
            //StringBuilder description = new StringBuilder();

            AlfredResponse response = await m_Engine.SearchAsync(i_SelectedText);

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
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SearchBySelectedTextAsync(searchComboBox.Text);
            }
        }
    }
}