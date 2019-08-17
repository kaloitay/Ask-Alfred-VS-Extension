using System.Threading.Tasks;

// This interface uses for logic only
namespace Ask_Alfred.Infrasructure.Interfaces
{
    public interface IWebDataSource
    {
        Task ParseDataAsync();
        IPage Page { get;  }

        //static ESupportedProgrramingLanguages SupportedLanguages { get; }

        //WebDataParser Parser { get; }
    }
}
