using Ask_Alfred.Infrastructure.Interfaces;

namespace Ask_Alfred.UI.VisualStudioApi
{
    internal class AlfredInput : IAlfredInput
    {
        public string Description { get; set; }
        public string ErrorCode { get; set; }

        public AlfredInput()
        {

        }
        public AlfredInput(string i_Description, string i_ErrorCode)
        {
            Description = i_Description;
            ErrorCode = i_ErrorCode;
        }
    }
}
