using System.Threading;
using System.Threading.Tasks;

namespace EasyCiteLib.Interface.Queue
{
    public interface IQueueManager
    {
        Task ScrapeArticleAsync(string documentId, CancellationToken cancelScrape = default);
    }
}