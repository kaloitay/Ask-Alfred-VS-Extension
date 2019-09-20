namespace Ask_Alfred.Infrastructure.Interfaces
{
    public interface IAlfredInput
    {
        string Description { get; set; }
        string ErrorCode { get; set; }
        string ProjectType { get; set; }
    }
}
