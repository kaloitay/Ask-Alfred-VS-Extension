using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System;
using System.Reflection;

namespace Ask_Alfred.UI.VisualStudioApi
{// *** TODO: should be internal class ... ?
    public static class VisualStudioHandler
    {
        public enum eErrorListValue : int
        {
            [StringValue("text")]
            Description,

            [StringValue("errorcode")]
            ErrorCode,

            [StringValue("line")]
            Line
        }

        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static bool UserIsOnEditor()
        {
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));

            var what = dte.ActiveWindow;//.ObjectKind
            
            return false;
        }

        public static bool UserIsOnErrorListToolWindow()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            //dte.ToolWindows.ErrorList
            return false;
        }

        public static ErrorItem GetErrorItemByActiveDocumentLineNumber(int i_LineNumber)
        {
            ErrorItem errorItem = null;

            var dte = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(DTE));
            var errorList = dte.ToolWindows.ErrorList;

            if (errorList != null && errorList.ErrorItems != null)
            {
                for (int i = 1; i <= errorList.ErrorItems.Count; i++)
                {
                    int line = errorList.ErrorItems.Item(i).Line;

                    if (line == i_LineNumber)
                        errorItem = errorList.ErrorItems.Item(i);//.Description;
                }
            }

            return errorItem;
        }

        public static ErrorItem GetErrorItemByActiveDocumentCurrentLine()
        {
            dynamic selection = GetActiveDocumentTextSelection();

            int currentLine = selection.CurrentLine;

            return GetErrorItemByActiveDocumentLineNumber(currentLine);
        }

        public static string GetCurrentLineErrorCode()
        {
            ErrorItem errorItem = GetErrorItemByActiveDocumentCurrentLine();
            IErrorList errorList = GetErrorList();
            string errorCode = null;

            var type = errorItem.GetType();

            foreach (ITableEntry entry in errorList.TableControl.Entries)
            {
                int entryLineNumber;

                bool hasValue = entry.TryGetValue(eErrorListValue.Line.GetStringValue(), out entryLineNumber);

                // *** bugs potential - many errors in the same line .....
                if (hasValue && entryLineNumber + 1 == errorItem.Line)
                {
                    entry.TryGetValue(eErrorListValue.ErrorCode.GetStringValue(), out errorCode);
                }
            }

            return errorCode;
        }

        public static bool IsCurrentLineHasError()
        {
            return !String.IsNullOrEmpty(GetCurrentLineErrorDescription());
        }

        public static string GetCurrentLineErrorDescription()
        {
            //  ThreadHelper.ThrowIfNotOnUIThread();

            ErrorItem errorItem = GetErrorItemByActiveDocumentCurrentLine();

            return errorItem != null ? errorItem.Description : null;
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

        /// <summary>
        /// Returns true if there is a selected error in error list tool window, false otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool HasSelectedError()
        {
            return (GetSelectedErrorListEntry() != null);
        }

        /// <summary>
        /// Gets the selected error value (by errorCode || description || error list window options)
        /// </summary>
        /// <param name="i_Value">
        /// param options -> description == "text", errorCode == "errorcode"
        /// </param>
        /// <returns></returns>
        public static string GetSelectedErrorValue(eErrorListValue i_Value)
        {
            string errorValue = null;

            ITableEntryHandle selected = GetSelectedErrorListEntry();

            if (selected != null)
            {
                object content;

                if (selected.TryGetValue(i_Value.GetStringValue(), out content))
                {
                    errorValue = (string)content;
                }

            }

            return errorValue;
        }

        private static ITableEntryHandle GetSelectedErrorListEntry()
        {
            IErrorList errorList = GetErrorList();

            ITableEntryHandle selected = errorList.TableControl.SelectedEntry;

            return selected;
        }
        // *** TODO: try to change from var to specific type
        private static IErrorList GetErrorList()
        {
            //     ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            var errorList = dte.ToolWindows.ErrorList as IErrorList;

            return errorList;
        }
    }
}
