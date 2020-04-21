using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Queue;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyCiteLib.Implementation.Queue
{
    public class QueueManager : IQueueManager, IAsyncDisposable
    {
        private const string _scrapeQueueName = "scrape";
        private const string _completeQueuePrefix = "scrape-complete";

        private readonly ILogger<QueueManager> _logger;
        private readonly IGenericDataContextAsync<ProjectReference> _projectReferenceContext;

        private readonly string _serviceBusConnectionString;

        readonly SemaphoreSlim _requestSenderCreateLock = new SemaphoreSlim(1, 1);
        private RequestReplySender _requestSender;
        private string _completeQueueName;

        private readonly ConcurrentDictionary<string, Task> _pendingScrapes = new ConcurrentDictionary<string, Task>();

        public QueueManager(IConfiguration configuration, ILogger<QueueManager> logger, IGenericDataContextAsync<ProjectReference> projectReferenceContext)
        {
            _serviceBusConnectionString = configuration["ServiceBusConnectionString"];
            _logger = logger;
            _projectReferenceContext = projectReferenceContext;
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
            try
            {
                await TryCreateRequestSenderAsync();

                var messageContent = new ScrapeMessage
                {
                    DocumentId = documentId,
                    Depth = depth,
                    ReplyTo = _completeQueueName
                };

                var messageJson = JsonConvert.SerializeObject(messageContent);

                var message = new Message(Encoding.UTF8.GetBytes(messageJson))
                {
                    TimeToLive = TimeSpan.FromMinutes(10),
                    MessageId = Guid.NewGuid().ToString()
                };

                _logger.LogInformation($"Sending scrape message for document {documentId}");
                await _requestSender.RequestAsync(message, rsp => Task.FromResult(true), new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

                await MarkReferencesNotPendingAsync(documentId);

                _logger.LogInformation($"Finished scrape for document {documentId}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Scrape task for document {documentId} took too long.");
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

        async Task MarkReferencesNotPendingAsync(string referenceId)
        {
            await foreach (var pr in _projectReferenceContext.DataSet.Where(pr => pr.ReferenceId == referenceId).AsAsyncEnumerable())
                pr.IsPending = false;

            await _projectReferenceContext.SaveChangesAsync();
        }

        async Task TryCreateRequestSenderAsync()
        {
            try
            {
                await _requestSenderCreateLock.WaitAsync();
                
                if (_requestSender != null)
                    return;

                await CreateNewCompleteQueueAsync();

                var sender = new MessageSender(_serviceBusConnectionString, _scrapeQueueName);
                var receiver = new MessageReceiver(_serviceBusConnectionString, _completeQueueName);

                _requestSender = new RequestReplySender(sender, receiver);
            }
            finally
            {
                _requestSenderCreateLock.Release();
            }
        }

        async Task CreateNewCompleteQueueAsync()
        {
            var managementClient = new ManagementClient(_serviceBusConnectionString);

            static string GetQueueName() => $"{_completeQueuePrefix}-{Guid.NewGuid():N}";

            try
            {
                while (string.IsNullOrEmpty(_completeQueueName))
                {
                    string queueName = GetQueueName();

                    if (await managementClient.QueueExistsAsync(queueName))
                        continue;

                    await managementClient.CreateQueueAsync(new QueueDescription(queueName)
                    {
                        EnablePartitioning = false,
                        AutoDeleteOnIdle = TimeSpan.FromHours(1),
                        DefaultMessageTimeToLive = TimeSpan.FromMinutes(5)
                    });
                    
                    _completeQueueName = queueName;
                    _logger.LogInformation($"Created queue {_completeQueueName}.");
                }
            }
            finally
            {
                await managementClient.CloseAsync();
            }
        }

        async Task DeleteCompleteQueueAsync(string queueName)
        {
            var managementClient = new ManagementClient(_serviceBusConnectionString);

            try
            {
                await managementClient.DeleteQueueAsync(queueName);
            }
            finally
            {
                await managementClient.CloseAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_requestSender != null)
            {
                await DeleteCompleteQueueAsync(_completeQueueName);
                _logger.LogInformation($"Deleting queue {_completeQueueName}.");
                await _requestSender.DisposeAsync();
            }
        }

        class ScrapeMessage
        {
            [JsonProperty("documentId")]
            public string DocumentId { get; set; }

            [JsonProperty("depth")]
            public int Depth { get; set; }
            
            [JsonProperty("replyTo")]
            public string ReplyTo { get; set; }
        }
    }
}