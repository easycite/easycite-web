using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Queue;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;

namespace EasyCiteLib.Implementation.Queue
{
    public class QueueManager : IQueueManager, IAsyncDisposable
    {
        private const string _scrapeQueueName = "scrape";
        private const string _completeQueueName = "scrape-complete";

        private readonly ISenderClient _senderClient;
        private readonly IReceiverClient _receiverClient;

        private readonly RequestReplySender _requestSender;

        public QueueManager(IConfiguration configuration)
        {
            string serviceBusConnectionString = configuration["ServiceBusConnectionString"];

            _senderClient = new MessageSender(serviceBusConnectionString, _scrapeQueueName);
            _receiverClient = new MessageReceiver(serviceBusConnectionString, _completeQueueName);

            _requestSender = new RequestReplySender(_senderClient, _receiverClient);
        }

        public async Task ScrapeArticleAsync(string documentId, CancellationToken cancelScrape = default)
        {
            var message = new Message(Encoding.UTF8.GetBytes(documentId))
            {
                TimeToLive = TimeSpan.FromMinutes(5),
                MessageId = Guid.NewGuid().ToString()
            };

            await _requestSender.RequestAsync(message, rsp => Task.FromResult(true), cancelScrape);
        }

        public async ValueTask DisposeAsync()
        {
            await Task.WhenAll(_senderClient.CloseAsync(), _receiverClient.CloseAsync());
        }
    }
}