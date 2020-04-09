using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace EasyCiteLib.Implementation.Queue
{
    public class RequestReplySender
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<Message>> _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<Message>>();

        private readonly ISenderClient _sender;
        private readonly IReceiverClient _receiver;

        public RequestReplySender(ISenderClient sender, IReceiverClient receiver)
        {
            _sender = sender;
            _receiver = receiver;

            _receiver.RegisterMessageHandler(OnMessageReceived,
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    AutoComplete = false
                });
        }

        public async Task RequestAsync(Message message, Func<Message, Task<bool>> replyHandler, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<Message>();

            if (!_pendingRequests.TryAdd(message.MessageId, tcs))
                throw new InvalidOperationException("Request with the MessageId is already pending.");

            token.Register(tcs.SetCanceled);

            await _sender.SendAsync(message);

            do
            {
                Message reply = await tcs.Task;
                try
                {
                    bool processed = await replyHandler(reply);
                    if (processed)
                    {
                        _pendingRequests.TryRemove(reply.CorrelationId, out _);
                    }
                }
                catch
                {
                    await _receiver.AbandonAsync(reply.SystemProperties.LockToken);
                    _pendingRequests.TryRemove(reply.CorrelationId, out _);
                    throw;
                }
            } while (token.IsCancellationRequested);
        }

        private async Task OnMessageReceived(Message message, CancellationToken cancellation)
        {
            if (_pendingRequests.TryGetValue(message.CorrelationId, out TaskCompletionSource<Message> tcs))
                tcs.SetResult(message);
            else
                await _receiver.DeadLetterAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            return Task.CompletedTask;
        }
    }
}