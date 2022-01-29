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
    [Route("api/GetCalculations")]
    public async Task<IActionResult> GetCalculations()
    {
        const int numberOfItemsToPublish = 10;
        _logger.LogInformation($"Start publish");
        var numbers = Enumerable.Range(1, numberOfItemsToPublish);
        var requests = numbers.Select(x => new CalculationRequest { Number = x });

        using var client = new DaprClientBuilder().Build();
        var data = new CalculationRequest() { Number = 7 };
        await client.PublishEventAsync("pubsub", "receivenumber", data);
        _logger.LogInformation($"Publish completed");

        return Accepted();
    }

    [Topic("pubsub", "receiveresult")]
    [HttpPost]
    [Route("receiveresult")]
    public IActionResult ProcessOrder([FromBody] CalculationResponse response)
    {
        //Process order placeholder
        string headers = String.Join(",", _httpContext.HttpContext!.Request.Headers.Select(x => x.Value));
        _logger.LogInformation($"Headers: {headers}");
        _logger.LogInformation($"Response with {response.Number} returned result {response.Result}");
        return Ok();
    }
}
