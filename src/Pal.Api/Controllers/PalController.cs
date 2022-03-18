namespace Pal.Api.Controllers;

using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Pal.Api.Services;
using Palprimes.Common;

[ApiController]
[Route("[controller]")]
public class PalController : ControllerBase
{
    private readonly ILogger<PalController> _logger;
    private readonly DaprClient _daprClient;
    private readonly DefaultPalNumberStrategy _strategy;

    private const int decimalBase = 10;

    public PalController(DaprClient daprClient, DefaultPalNumberStrategy strategy, ILogger<PalController> logger)
    {
        this._strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        this._daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Topic("pubsub", "receivenumber")]
    [HttpPost]
    public async Task<IActionResult> ReceiveNumber([FromBody] CalculationRequest request)
    {
        _logger.LogInformation($"Received {request.Number}");

        var result = await _strategy.IsPalindromicAsync(request.Number, decimalBase);
        var response = new CalculationResponse
        {
            ClientId = request.ClientId,
            Number = request.Number,
            Result = result,
            Type = CalculationResultType.Pal
        };

        await _daprClient.PublishEventAsync("pubsub", "results", response);

        _logger.LogInformation($"Sent response {response.Number}/{response.Result}");

        return Ok();
    }
}
