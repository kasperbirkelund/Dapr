using System.Collections.Concurrent;
using Palprimes.Api.Model;

namespace Palprimes.Api.Services
{
    public class NotificationService
    {
        private ConcurrentDictionary<string, Action<PalprimesResult>> _subscriptions = new ConcurrentDictionary<string, Action<PalprimesResult>>();
        private readonly ILogger<NotificationService> _logger;


        public NotificationService(ILogger<NotificationService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Subscribe(string clientId, Action<PalprimesResult> action)
        {
            _subscriptions.AddOrUpdate(clientId, action, (k, v) => v = action);
            _logger.LogInformation($"Subscribed {clientId}");
        }

        public void Unsubscribe(string clientId)
        {
            _subscriptions.TryRemove(clientId, out var action);
            _logger.LogInformation($"Unsubscribed {clientId}");
        }

        public void Notify(string clientId, PalprimesResult result)
        {
            if (_subscriptions.TryGetValue(clientId, out var action))
            {
                _logger.LogInformation($"Notifying {clientId}/{result.Number}");
                action.Invoke(result);
            }
            else
            {
                _logger.LogWarning($"No subscription found for {clientId}/{result.Number}");
            }
        }
    }
}