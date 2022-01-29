using System.Text;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    [Route("api/publish")]
    public async Task<IActionResult> Puhlish()
    {
        //const int numberOfItemsToPublish = 1000;
        //_logger.LogInformation($"Start publish");

        //using (var httpClient = new HttpClient())
        //{
        //    foreach (var VARIABLE in COLLECTION)
        //    {
        //        var result = await httpClient.PostAsync(
        //            "http://localhost:3500/v1.0/publish/ordertopic",
        //            new StringContent(
        //                System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json")
        //        );
        //    }

        //    _logger.LogInformation($"{order.id} published with status {result.StatusCode}!");
        //}

        //return Ok();
        throw new NotImplementedException();
    }

    [HttpPost(Name = "consume")]
    public Task<ActionResult> Publish()
    {
        _logger.LogInformation("Publish started");
        throw new NotImplementedException();
    }
}
