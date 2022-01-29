using System.Text;
using System.Text.Json;
using Dapr;
using Microsoft.AspNetCore.Mvc;

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

    [Topic("receivenumber")]
    [HttpPost()]
    [Route("receivenumber")]
    public async Task<IActionResult> ReceiveNumber([FromBody] int number)
    {
        _logger.LogInformation($"Received {number}");

        using (var httpClient = new HttpClient())
        {
            var result = await httpClient.PostAsync(
                 "http://localhost:3500/v1.0/publish/receiveresult",
                 new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json")
            );

            _logger.LogInformation($"Order with id {order.id} published with status {result.StatusCode}!");
        }


        return Ok();
    }
}
