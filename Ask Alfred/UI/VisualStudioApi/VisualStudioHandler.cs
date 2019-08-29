using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ask_Alfred.UI.VisualStudioApi
{
    internal static class VisualStudioHandler
    {
        public static string GetErrorDescriptionByActiveDocumentLineNumber(int i_LineNumber)
        {
            string description = null;

            var dte = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(DTE));
            var errorList = dte.ToolWindows.ErrorList;

            for (int i = 1; i <= errorList.ErrorItems.Count; i++)
            {
                int line = errorList.ErrorItems.Item(i).Line;

                if (line == i_LineNumber)
                    description = errorList.ErrorItems.Item(i).Description;
            }

            return description;

            //for (int i = 1; i <= errorList.ErrorItems.Count; i++)
            //{
            //    // errorItem contains -> Description, FileName, Line, Column, Project
            //    var errorItem = errorList.ErrorItems.Item(i);

            //    description = errorItem.Description;
            //}

        }

        public static bool IsCurrentLineHasError()
        {
            return String.IsNullOrEmpty(GetCurrentLineErrorDescription());
        }

        public static string GetCurrentLineErrorDescription()
        {
          //  ThreadHelper.ThrowIfNotOnUIThread();

            dynamic selection = GetActiveDocumentTextSelection();

           int currentLine = selection.CurrentLine;

           return GetErrorDescriptionByActiveDocumentLineNumber(currentLine);
        }

        /// <summary>
        /// Gets the active document selection, or null if there isnt
        /// </summary>
        /// <returns></returns>
        public static dynamic GetActiveDocumentTextSelection()
        {
         //   ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            dynamic selection = null;

            if (dte.ActiveDocument != null)
            {
                selection = (TextSelection)dte.ActiveDocument.Selection;
                
            }

            return selection;
        }

        /// <summary>
        /// Gets the selected text from the active document
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedText()
        {
            var selection = GetActiveDocumentTextSelection();

            string selectedText = selection.Text;

            return selectedText;
        }
        /// <summary>
        /// Gets the selected or first error value (by errorCode || description || error list window options)
        /// </summary>
        /// <param name="i_Value">
        /// param options -> description == "text", errorCode == "errorcode"
        /// </param>
        /// <returns></returns>
        public static string GetSelectedOrFirstErrorValue(string i_Value)
        {
            string errorValue = null;

            IErrorList errorList = GetErrorList();
            var selected = errorList.TableControl.SelectedOrFirstEntry;

            if (selected != null)
            {
                object content;

                if (selected.TryGetValue(i_Value, out content))
                {
                    errorValue = (string)content;
                }

            }

            return errorValue;
        }

        public static string GetSelectedErrorValue(string i_Value)
        {
            string errorValue = null;

            IErrorList errorList = GetErrorList();
            var selected = errorList.TableControl.SelectedEntry;

            if (selected != null)
            {
                object content;

                if (selected.TryGetValue(i_Value, out content))
                {
                    errorValue = (string)content;
                }

            }

            return errorValue;
        }

        private static IErrorList GetErrorList()
        {
       //     ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            var errorList = dte.ToolWindows.ErrorList as IErrorList;

            return errorList;
        }
    }
}
