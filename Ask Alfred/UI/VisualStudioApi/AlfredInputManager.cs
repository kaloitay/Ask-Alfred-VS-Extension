namespace Ask_Alfred.UI.VisualStudioApi
{
    internal class AlfredInputManager // *** TODO: should be singleton !!!! 
    {
        internal AlfredInput GetInputForAlfred()
        {
            string errorDescription = null;
            string errorCode = null;

            // *** function for testing
            if (VisualStudioHandler.UserIsOnEditor())
            {
                int i = 0;
            }

            if (VisualStudioHandler.IsCurrentLineHasError())
            {
                errorDescription = VisualStudioHandler.GetCurrentLineErrorDescription();
                errorCode = VisualStudioHandler.GetCurrentLineErrorCode();
            }
            else if (VisualStudioHandler.HasSelectedError())
            {
                errorDescription = VisualStudioHandler.GetSelectedErrorValue(VisualStudioHandler.eErrorListValue.Description);
                errorCode = VisualStudioHandler.GetSelectedErrorValue(VisualStudioHandler.eErrorListValue.ErrorCode);
            }
            //else if (VisualStudioHandler.S)
            //string selectedText = VisualStudioHandler.GetSelectedText();
            //string selectedOrFirstErrorDescription = VisualStudioHandler.GetSelectedOrFirstErrorValue("text");
            //string selectedErrorCode = VisualStudioHandler.GetSelectedOrFirstErrorValue("errorcode");

            //if (!String.IsNullOrEmpty(selectedText))
            //{
            //    //(window as AskAlfredWindow).AutoSearchByText(selectedText);
            //}
            //if (!String.IsNullOrEmpty(selectedOrFirstErrorDescription))
            //{
            //   // (window as AskAlfredWindow).AutoSearchByText(selectedOrFirstErrorDescription);
            //}

            return new AlfredInput(errorDescription, errorCode);
        }
    }
}
