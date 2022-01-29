using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Palprimes.Common;

namespace Pal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PalController : ControllerBase
{
    private readonly ILogger<PalController> _logger;

    public PalController(ILogger<PalController> logger)
    {
        _logger = logger;
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

            var result = await httpClient.PostAsync(
                 "http://localhost:3500/v1.0/palprimesapi/receiveresult",
                 new StringContent(
                     JsonSerializer.Serialize(response),
                     Encoding.UTF8, MediaTypeNames.Application.Json)
            );

            _logger.LogInformation($"Sent response {response.Number}/{response.Result}");
        }
        return Ok();
    }
}
