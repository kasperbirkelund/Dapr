namespace Prime.Api.Controllers;

using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;
using Prime.Api.Services;

[ApiController]
[Route("[controller]")]
public class PrimeController : ControllerBase
{
    private readonly ILogger<PrimeController> _logger;
    private readonly DaprClient _daprClient;
    private readonly DefaultPrimeNumberStrategy _strategy;

    public PrimeController(DaprClient daprClient, DefaultPrimeNumberStrategy strategy, ILogger<PrimeController> logger)
    {
        this._daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        this._strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Topic(DaprConstants.KafkaPubSub, DaprConstants.PubSubTopics.ReceiveNumber)]
    [HttpPost]
    public async Task<IActionResult> ReceiveNumber([FromBody] CalculationRequest request)
    {
        _logger.LogInformation($"Received {request.Number}");
        var result = await _strategy.IsPrimeAsync(request.Number);

        var response = new CalculationResponse
        {
            ClientId = request.ClientId,
            Number = request.Number,
            Result = result,
            Type = CalculationResultType.Prime
        };

        await _daprClient.PublishEventAsync(DaprConstants.KafkaPubSub, DaprConstants.PubSubTopics.Results, response);

        _logger.LogInformation($"Sent response {response.Number}/{response.Result}");

        return Ok();
    }
}
