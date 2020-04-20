using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search.Export;

namespace EasyCiteLib.Interface.Search.Export
{
    public interface IGetSearchExportDataProcessor
    {
        Task<Results<ExportData>> GetAsync(int projectId);
    }
}