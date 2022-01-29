using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;

namespace Prime.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PrimeController : ControllerBase
{
    private readonly ILogger<PrimeController> _logger;
    private readonly DaprClient _daprClient;

    public PrimeController(DaprClient daprClient, ILogger<PrimeController> logger)
    {
        this._daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Topic("pubsub", "receivenumber")]
    [HttpPost]
    public async Task<IActionResult> ReceiveNumber([FromBody] CalculationRequest request)
    {
        _logger.LogInformation($"Received {request.Number}");

        var response = new CalculationResponse
        {
            Number = request.Number,
            Result = false
        };

        await _daprClient.PublishEventAsync("pubsub", "receiveprimes", response);

        _logger.LogInformation($"Sent response {response.Number}/{response.Result}");

        return Ok();
    }
}
