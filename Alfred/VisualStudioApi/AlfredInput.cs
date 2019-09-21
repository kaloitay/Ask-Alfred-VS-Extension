using Ask_Alfred.Infrastructure.Interfaces;

namespace Ask_Alfred.UI.VisualStudioApi
{
    public class AlfredInput : IAlfredInput
    {
        public string Description { get; set; }
        public string ErrorCode { get; set; }
        public string ProjectType { get; set; }

        public AlfredInput()
        {

        }
        public AlfredInput(string i_Description, string i_ErrorCode, string i_ProjectType)
        {
            Description = i_Description;
            ErrorCode = i_ErrorCode;
            ProjectType = i_ProjectType;
        }
    }
}
