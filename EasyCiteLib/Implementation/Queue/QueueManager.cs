using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Queue;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyCiteLib.Implementation.Queue
{
    public class QueueManager : IQueueManager, IAsyncDisposable
    {
        private const string _scrapeQueueName = "scrape";
        private const string _completeQueueName = "scrape-complete";

        private readonly ILogger<QueueManager> _logger;

        private readonly ISenderClient _senderClient;
        private readonly IReceiverClient _receiverClient;

        private readonly RequestReplySender _requestSender;

        private readonly ConcurrentDictionary<string, Task> _pendingScrapes = new ConcurrentDictionary<string, Task>();

        public QueueManager(IConfiguration configuration, ILogger<QueueManager> logger)
        {
            string serviceBusConnectionString = configuration["ServiceBusConnectionString"];
            _logger = logger;

            _senderClient = new MessageSender(serviceBusConnectionString, _scrapeQueueName);
            _receiverClient = new MessageReceiver(serviceBusConnectionString, _completeQueueName);

            _requestSender = new RequestReplySender(_senderClient, _receiverClient);
        }

        public void QueueArticleScrape(string documentId, int depth)
        {
            _pendingScrapes.GetOrAdd(documentId, ScrapeArticleAsync, depth);
        }

        public bool IsScrapePending(string documentId)
        {
            return _pendingScrapes.ContainsKey(documentId);
        }

        private async Task ScrapeArticleAsync(string documentId, int depth)
        {
            var messageContent = new ScrapeMessage
            {
                DocumentId = documentId,
                Depth = depth
            };

            var messageJson = JsonConvert.SerializeObject(messageContent);
            
            var message = new Message(Encoding.UTF8.GetBytes(messageJson))
            {
                TimeToLive = TimeSpan.FromMinutes(5),
                MessageId = Guid.NewGuid().ToString()
            };

            try
            {
                _logger.LogInformation($"Sending scrape message for document {documentId}");
                await _requestSender.RequestAsync(message, rsp => Task.FromResult(true), CancellationToken.None);
                _logger.LogInformation($"Finished scrape for document {documentId}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Scrape message failure.");
            }
            finally
            {
                _pendingScrapes.TryRemove(documentId, out _);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Task.WhenAll(_senderClient.CloseAsync(), _receiverClient.CloseAsync());
        }

        class ScrapeMessage
        {
            [JsonProperty("documentId")]
            public string DocumentId { get; set; }
            
            [JsonProperty("depth")]
            public int Depth { get; set; }
        }
    }
}