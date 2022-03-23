using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Palprimes.Api.Hubs;
using Palprimes.Api.Model;
using Palprimes.Api.Services;
using Palprimes.Common;

namespace Palprimes.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private const string StoreName = "statestore";
    private readonly NotificationService _notificationService;
    private readonly ILogger<CalculationController> _logger;
    private readonly StateManagementService _stateService;

    public CalculationController(StateManagementService stateService, NotificationService notificationService, ILogger<CalculationController> logger)
    {
        this._stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        this._notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("api/GetCalculations/{clientId}/{top}")]
    public async Task<IActionResult> GetCalculations(string clientId, int top)
    {
        _logger.LogInformation($"Start publish {top} numbers.");

        var numbers = Enumerable.Range(1, top);
        var requests = numbers.Select(x => new CalculationRequest { Number = x, ClientId = clientId });

        using var client = new DaprClientBuilder().Build();
        var requestTasks = requests.Select(request =>
        {
            return client.PublishEventAsync(DaprConstants.KafkaPubSub, DaprConstants.PubSubTopics.ReceiveNumber, request);
        });

        await Task.WhenAll(requestTasks);

        _logger.LogInformation($"Publish completed");

        return Accepted();
    }

    [Topic(DaprConstants.KafkaPubSub, DaprConstants.PubSubTopics.Results)]
    [HttpPost(DaprConstants.PubSubTopics.Results)]
    public async Task<IActionResult> ReceiveResults([FromBody] CalculationResponse response)
    {
        _logger.LogInformation($"Received response {response.Type}/{response.Number}/{response.Result}");

        var state = await _stateService.SetStateAsync(response);

        _logger.LogInformation($"State: {response.ClientId}/IsPal:${state.IsPal}/IsPrime:${state.IsPrime}.");

        if (state.IsPal.HasValue && state.IsPrime.HasValue)
        {
            _logger.LogInformation($"Response: {response.ClientId}/{response.ClientId}/IsPal:${state.IsPal.Value}/IsPrime:${state.IsPrime.Value}");
            _notificationService.Notify(response.ClientId, state);

            //We could remove this and start caching and serving results also directly from state store.
            await _stateService.ResetStateAsync(state);
        }

        return Ok();
    }


}
