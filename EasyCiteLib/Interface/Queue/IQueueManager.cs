namespace EasyCiteLib.Interface.Queue
{
    public interface IQueueManager
    {
        void QueueArticleScrape(string documentId, int depth);
        bool IsScrapePending(string documentId);
    }
}