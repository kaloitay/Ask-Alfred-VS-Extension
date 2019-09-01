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

    /// <summary>
    /// Interaction logic for AskAlfredWindowControl.
    /// </summary>
    public partial class AskAlfredWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AskAlfredWindowControl"/> class.
        /// </summary>
        public AskAlfredWindowControl()
        {
            InitializeComponent();
            InitializeAskAlfredWindow();
        }

        private void InitializeAskAlfredWindow()
        {
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
            // else...
        }

        private void createWindowResult(IPage i_Page)
        {
            AskAlfredResultUIElement askAlfredResultUIElement = new AskAlfredResultUIElement(i_Page, this.Resources);
            resultsListView.Items.Add(askAlfredResultUIElement.dockPanel);
        }

        private void searchIsFinished()
        {
            searchComboBox.IsEnabled = true;
        }
        private void timeoutExpiredHandler()
        {
            //searchComboBox.IsEnabled = true;
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
                ClearAndSearch(errorDescription);
            }
        }

        private void ClearAndSearch(string i_ErrorDescription)
        {
            searchComboBox.IsEnabled = false;
            resultsListView.Items.Clear();
            searchComboBox.Text = i_ErrorDescription;
            searchingForTextBlock.Text = "Searching For '"+ i_ErrorDescription +"'";
            AskAlfredSearchAsync(i_ErrorDescription);
        }

        public async System.Threading.Tasks.Task AskAlfredSearchAsync(string i_SelectedText)
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
            if (e.Key == Key.Enter && searchComboBox.IsEnabled == true)
            {
                ClearAndSearch(searchComboBox.Text);
            }
        }

        //private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Debug.Write("@@@@@@@@@@@@@@" + (sender as Control).Name + "@@@@@@@@@@@@@@@");
        //    System.Diagnostics.Process.Start((sender as Control).Name);
        //}
    }
}