using System.Threading.Tasks;
using EasyCiteLib.DataModel;

namespace EasyCiteLib.Interface
{
    public interface IGetExampleDataProcessor
    {
        Task<ExampleData> Get(int id);
    }
}