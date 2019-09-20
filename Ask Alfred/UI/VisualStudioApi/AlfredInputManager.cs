using Microsoft.VisualStudio.Shell;
using System;

namespace Ask_Alfred.UI.VisualStudioApi
{
    public sealed class AlfredInputManager
    {
        private static readonly Lazy<AlfredInputManager> lazy = new Lazy<AlfredInputManager>(() => new AlfredInputManager());
        public static AlfredInputManager Instance { get { return lazy.Value; } }
        public AlfredInput GetInputFromSelectedError()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string errorDescription = null;
            string errorCode = null;
            string projetType = VisualStudioHandler.GetProjectTypeAsString();

            if (VisualStudioHandler.HasSelectedError())
            {
                errorDescription = VisualStudioHandler.GetValueFromSelectedError("text");
                errorCode = VisualStudioHandler.GetValueFromSelectedError("errorcode");
            }

            return new AlfredInput(errorDescription, errorCode, projetType);
        }

        public AlfredInput GetInputForAlfredWindowSearchBar(string i_SearchKey)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string searchKey = i_SearchKey;
            string projetType = VisualStudioHandler.GetProjectTypeAsString();

            return new AlfredInput(searchKey, null, projetType);
        }
    }
}
