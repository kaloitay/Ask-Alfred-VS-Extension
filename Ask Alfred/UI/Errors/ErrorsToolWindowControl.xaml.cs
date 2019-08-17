namespace Ask_Alfred.UI.Errors
{
    using Ask_Alfred.Infrasructure;
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using EnvDTE;
    using Package = Microsoft.VisualStudio.Shell.Package;
    using System.Text;
    using Ask_Alfred.Infrasructure.Interfaces;
    using System;
    using System.Linq;
    using Ask_Alfred.Objects;

    /// <summary>
    /// Interaction logic for ErrorsToolWindowControl.
    /// </summary>
    public partial class ErrorsToolWindowControl : UserControl
    {
        AlfredEngine m_Engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsToolWindowControl"/> class.
        /// </summary>
        public ErrorsToolWindowControl()
        {
            this.InitializeComponent();
            m_Engine = new AlfredEngine();
            m_Engine.OnPageAdded += pageAddedHandler;
            m_Engine.OnTimeoutExpired += timeoutExpiredHandler;
        }

        private void pageAddedHandler(IPage page)
        {
            StackoverflowPage stackoverflowPage = page as StackoverflowPage;
            if (stackoverflowPage != null)
            {
                this.dataGridViewPages.Items.Add(stackoverflowPage);
            }
            // else...
        }
        private void timeoutExpiredHandler()
        {
            System.Windows.MessageBox.Show("Timeout Expired", "Timeout Expired",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string errorDescription = getErrorDescription();
            string errorCode = getErrorCode();

            if (errorDescription == null)
            {
                dataGridViewPages.Items.Add("No errors detected.");
            }
            else
            {
                dataGridViewPages.Items.Clear();
                getResultsAsText(errorDescription); // TODO: change method name
            }
        }

        private void getResultsAsText(string i_ErrorDescription)
        {
            // TODO: find a normal way to do it
            ESupportedProgrramingLanguages currentLanguage = ESupportedProgrramingLanguages.CSharp;

            StringBuilder description = new StringBuilder();
            AlfredResponse response = m_Engine.Search(i_ErrorDescription);

            // IPage or IWebDataSource?
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                 .Where(mytype => mytype.GetInterfaces().Contains(typeof(IPage))))
            {
                var props = mytype.GetFields();

                //Console.WriteLine(props[0].GetValue(null));
            }
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

        private void DataGridViewPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}