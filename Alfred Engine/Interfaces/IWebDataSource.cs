using System.Threading.Tasks;

namespace Ask_Alfred.Infrastructure.Interfaces
{
    public interface IWebDataSource
    {
        Task<IPage> ParseDataAndGetPageAsync();
        int GoogleResultIndex { get; set; }
    }
}
