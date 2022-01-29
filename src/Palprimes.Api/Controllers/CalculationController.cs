using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;

namespace Palprimes.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private readonly ILogger<CalculationController> _logger;

    public CalculationController(ILogger<CalculationController> logger)
    {
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

        using (var httpClient = new HttpClient())
        {
            foreach (var request in requests)
            {
                var result = await httpClient.PostAsync(
                    "http://localhost:3500/v1.0/palapi/receivenumber",
                    new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json)
                );
                _logger.LogInformation($"{request.Number} is published with status {result.StatusCode}!");
            }
        }
        return Accepted();
    }

    [Topic("pubsub", "receiveresult")]
    [HttpPost]
    [Route("receiveresult")]
    public async Task<IActionResult> ProcessOrder([FromBody] CalculationResponse response)
    {
        //Process order placeholder

        _logger.LogInformation($"Response with {response.Number} returned result {response.Result}");
        return Ok();
    }
}
