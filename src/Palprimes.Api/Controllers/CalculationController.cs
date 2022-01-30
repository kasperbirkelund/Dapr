using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;

namespace Palprimes.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private readonly ILogger<CalculationController> _logger;
    private readonly IHttpContextAccessor _httpContext;

    public CalculationController(IHttpContextAccessor httpContext, ILogger<CalculationController> logger)
    {
        this._httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        _logger = logger;
    }

    [HttpGet]
    [Route("api/GetCalculations/{top}")]
    public async Task<IActionResult> GetCalculations(int top)
    {        
        _logger.LogInformation($"Start publish {top} numbers.");
        var numbers = Enumerable.Range(1, top);
        var requests = numbers.Select(x => new CalculationRequest { Number = x });

        using var client = new DaprClientBuilder().Build();
        var requestTasks = requests.Select( x =>
        {            
            return client.PublishEventAsync("pubsub", "receivenumber", x);
        });

        await Task.WhenAll(requestTasks);

        _logger.LogInformation($"Publish completed");

        return Accepted();
    }

    [Topic("pubsub", "receivepals")]
    [HttpPost("receivepals")]
    public IActionResult ReceivePals([FromBody] CalculationResponse response)
    {
        _logger.LogInformation($"Pals response with {response.Number} returned result {response.Result}");
        return Ok();
    }

    [Topic("pubsub", "receiveprimes")]
    [HttpPost("receiveprimes")]
    public IActionResult ReceivePrimes([FromBody] CalculationResponse response)
    {
        _logger.LogInformation($"Prime response with {response.Number} returned result {response.Result}");
        return Ok();
    }
}
