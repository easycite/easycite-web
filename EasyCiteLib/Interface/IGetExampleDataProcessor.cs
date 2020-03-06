using System.Threading.Tasks;
using EasyCiteLib.Models;

namespace EasyCiteLib.Interface
{
    public interface IGetExampleDataProcessor
    {
        Task<ExampleData> GetAsync(int id);
    }
}