using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ask_Alfred.UI.VisualStudioApi
{
    public static class VisualStudioHandler
    {
        private static readonly Dictionary<string, string> r_ProjectGuids;

        static VisualStudioHandler()
        {
            r_ProjectGuids = new Dictionary<string, string>
            {
                { "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", "C#"},
                { "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}", "VB.NET"},
                { "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}", "C++"},
                { "{F2A71F9B-5D33-465A-A702-920D77279786}", "F#"},
                { "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}", "J#"},
                { "{262852C6-CD72-467D-83FE-5EEB1973A190}", "JScript"},
                { "{349C5851-65DF-11DA-9384-00065B846F21}", "Web Application"},
                { "{E24C65DC-7377-472B-9ABA-BC803B73C61A}", "Web Site"},
                { "{F135691A-BF7E-435D-8960-F99683D2D49C}", "Distributed System"},
                { "{3D9AD99F-2412-4246-B90B-4EAA41C64699}", "Windows Communication Foundation (WCF)"},
                { "{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}", "Windows Presentation Foundation (WPF)"},
                { "{C252FEB5-A946-4202-B1D4-9916A0590387}", "Visual Database Tools"},
                { "{A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124}", "Database"},
                { "{4F174C21-8C12-11D0-8340-0000F80270F8}", "Database (other project types)"},
                { "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}", "Test"},
                { "{20D4826A-C6FA-45DB-90F4-C717570B9F32}", "Legacy (2003) Smart Device (C#)"},
                { "{CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8}", "Legacy (2003) Smart Device (VB.NET)"},
                { "{4D628B5B-2FBC-4AA6-8C16-197242AEB884}", "Smart Device (C#)"},
                { "{68B1623D-7FB9-47D8-8664-7ECEA3297D4F}", "Smart Device (VB.NET)"},
                { "{14822709-B5A1-4724-98CA-57A101D1B079}", "Workflow (C#)"},
                { "{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}", "Workflow (VB.NET)"},
                { "{06A35CCD-C46D-44D5-987B-CF40FF872267}", "Deployment Merge Module"},
                { "{3EA9E505-35AC-4774-B492-AD1749C4943A}", "Deployment Cab"},
                { "{978C614F-708E-4E1A-B201-565925725DBA}", "Deployment Setup"},
                { "{AB322303-2255-48EF-A496-5904EB18DA55}", "Deployment Smart Device Cab"},
                { "{A860303F-1F3F-4691-B57E-529FC101A107}", "Visual Studio Tools for Applications (VSTA)"},
                { "{BAA0C2D2-18E2-41B9-852F-F413020CAA33}", "Visual Studio Tools for Office (VSTO)"},
                { "{F8810EC1-6754-47FC-A15F-DFABD2E3FA90}", "SharePoint Workflow"},
                { "{6D335F3A-9D43-41b4-9D22-F6F17C4BE596}", "XNA (Windows)"},
                { "{2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}", "XNA (XBox)"},
                { "{D399B71A-8929-442a-A9AC-8BEC78BB2433}", "XNA (Zune)"},
                { "{EC05E597-79D4-47f3-ADA0-324C4F7C7484}", "SharePoint (VB.NET)"},
                { "{593B0543-81F6-4436-BA1E-4747859CAAE2}", "SharePoint (C#)"},
                { "{A1591282-1198-4647-A2B1-27E5FF5F6F3B}", "Silverlight"},
                { "{603C0E0B-DB56-11DC-BE95-000D561079B0}", "ASP.NET MVC 1.0"},
                { "{F85E285D-A4E0-4152-9332-AB1D724D3325}", "ASP.NET MVC 2.0"},
                { "{E53F8FEA-EAE0-44A6-8774-FFD645390401}", "ASP.NET MVC 3.0"},
                { "{E3E379DF-F4C6-4180-9B81-6769533ABE47}", "ASP.NET MVC 4.0"},
                { "{82B43B9B-A64C-4715-B499-D71E9CA2BD60}", "Extensibility"},
                { "{76F1466A-8B6D-4E39-A767-685A06062A39}", "Store App Windows Phone 8.1"},
                { "{C089C8C0-30E0-4E22-80C0-CE093F111A43}", "Store App Windows Phone 8.1 Silverlight (C#)"},
                { "{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}", "Store App Windows Phone 8.1 Silverlight (VB.NET)"},
                { "{BC8A1FFA-BEE3-4634-8014-F334798102B3}", "Store App Windows 8.1"},
                { "{D954291E-2A0B-460D-934E-DC6B0785DB48}", "Store App Universal"},
                { "{786C830F-07A1-408B-BD7F-6EE04809D6DB}", "Store App Portable Universal"},
                { "{8BB0C5E8-0616-4F60-8E55-A43933E57E9C}", "LightSwitch"},
                { "{581633EB-B896-402F-8E60-36F3DA191C85}", "LightSwitch Project"},
                { "{C1CDDADD-2546-481F-9697-4EA41081F2FC}", "Office/SharePoint App"}
            };
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
        public static bool IsCurrentLineHasError()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return getErrorListByCurrentLine().Count > 0;
        }
        public static string GetValueFromSelectedError(string i_Value)
        {
            ITableEntryHandle selected = getSelectedErrorListEntry();
            return GetStringFromTableEntry(selected, i_Value);
        }
        public static string GetStringFromTableEntry(ITableEntryHandle i_TableEntry, string i_Value)
        {
            string errorValue = null;
            i_TableEntry?.TryGetValue(i_Value, out errorValue);

            return errorValue;
        }

        public static List<VisualStudioErrorCodeItem> GetCurrentLineErrorList()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string errorCode;
            List<VisualStudioErrorCodeItem> visualStudioErrorCodeList = new List<VisualStudioErrorCodeItem>();
            List<ErrorItem> errorItemsList = getErrorListByCurrentLine();
            IErrorList errrorList = getErrorList();
            IEnumerable<ITableEntryHandle> tableEntries = errrorList.TableControl.Entries;

            foreach (ErrorItem item in errorItemsList)
            {
                errorCode = getErrorCodeFromErrorItem(tableEntries, item);

                visualStudioErrorCodeList.Add(new VisualStudioErrorCodeItem
                {
                    Description = item.Description,
                    Line = item.Line,
                    Column = item.Column,
                    File = item.FileName,
                    Project = item.Project,
                    ErrorCode = errorCode
                });
            }

            return visualStudioErrorCodeList;
        }

        private static string getErrorCodeFromErrorItem(IEnumerable<ITableEntryHandle> i_TableEntries, ErrorItem i_ErrorItem)
        {
            string errorCode = null;

            foreach (var tableEntry in i_TableEntries)
            {
                if (i_ErrorItem.Description == GetStringFromTableEntry(tableEntry, "text"))
                    errorCode = GetStringFromTableEntry(tableEntry, "errorcode");
            }

            return errorCode;
        }

        public static string GetProjectTypeAsString()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            Project activeProject = getActiveProject(dte);

            r_ProjectGuids.TryGetValue(activeProject.Kind, out string projectType);

            return projectType;
        }
        public static bool HasSelectedError()
        {
            return getSelectedErrorListEntry() != null;
        }
        public static string GetCurrentLineSelectedText()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            dynamic selection = null;

            if (dte.ActiveDocument != null)
                selection = (TextSelection)dte.ActiveDocument.Selection;

            return selection.Text;
        }

        private static List<ErrorItem> getErrorListByCurrentLine()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            int currentLine = getCurrentLineNumber();

            return getErrorListByLineNumber(currentLine);
        }
        private static List<ErrorItem> getErrorListByLineNumber(int i_LineNumber)
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            List<ErrorItem> lineErrorList = new List<ErrorItem>();

            ErrorList errorList = dte.ToolWindows.ErrorList;

            if (errorList != null && errorList.ErrorItems != null)
            {
                for (int i = 1; i <= errorList.ErrorItems.Count; i++)
                {
                    if (errorList.ErrorItems.Item(i).Line == i_LineNumber)
                        lineErrorList.Add(errorList.ErrorItems.Item(i));
                }
            }

            return lineErrorList;
        }
        private static int getCurrentLineNumber()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            dynamic selection = null;

            if (dte.ActiveDocument != null)
                selection = (TextSelection)dte.ActiveDocument.Selection;

            return selection.CurrentLine;
        }
        private static ITableEntryHandle getSelectedErrorListEntry()
        {
            IErrorList errorList = getErrorList();
            ITableEntryHandle selected = errorList.TableControl.SelectedEntry;

            return selected;
        }
        private static IErrorList getErrorList()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            IErrorList errorList = dte.ToolWindows.ErrorList as IErrorList;

            return errorList;
        }
        private static Project getActiveProject(DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }
    }
}
