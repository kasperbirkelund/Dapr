namespace Palprimes.Api.Hubs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Palprimes.Api.Services;

public class NotificationHub : Hub
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationService _notificationService;
    private readonly ILogger<NotificationHub> _logger;

    private string ConnectionId => Context.ConnectionId;

    public NotificationHub(IHubContext<NotificationHub> hubContext, NotificationService notificationService, ILogger<NotificationHub> logger)
    {
        this._hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        this._notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ActionResult> Subscribe(string clientId)
    {
        _logger.LogInformation($"Subscribing to {clientId}");

        await Groups.AddToGroupAsync(ConnectionId, clientId);

        _notificationService.Subscribe(clientId, async result =>
        {
            await _hubContext.Clients.Groups(clientId).SendAsync("result", result);
        });

        return new NoContentResult();
    }

    public async Task<ActionResult> Unsubscribe(string clientId)
    {
        _logger.LogInformation($"Unsubscribing from {clientId}");

        await Groups.RemoveFromGroupAsync(ConnectionId, clientId);
        
        _notificationService.Unsubscribe(clientId);
        
        return new NoContentResult();
    }
}
