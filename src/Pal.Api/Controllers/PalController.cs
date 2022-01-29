using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;

namespace Pal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PalController : ControllerBase
{
    private readonly ILogger<PalController> _logger;
    private readonly DaprClient _daprClient;

    public PalController(DaprClient daprClient, ILogger<PalController> logger)
    {
        this._daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Topic("pubsub", "receivenumber")]
    [HttpPost]
    //[Route("receivenumber")]
    public async Task<IActionResult> ReceiveNumber([FromBody] CalculationRequest request)
    {
        _logger.LogInformation($"Received {request.Number}");

        using (var httpClient = new HttpClient())
        {
            var response = new CalculationResponse
            {
                Number = request.Number,
                Result = true
            };

            await _daprClient.PublishEventAsync("pubsub", "receiveresult", response);
            
            _logger.LogInformation($"Sent response {response.Number}/{response.Result}");
        }
        return Ok();
    }
}
