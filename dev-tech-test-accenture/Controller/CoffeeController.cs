using Microsoft.AspNetCore.Mvc;
using dev_tech_test_accenture.Models;
using dev_tech_test_accenture.Services;

namespace dev_tech_test_accenture.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly ICoffeeService _coffeeService;
    private readonly IWeatherService _weatherService;

    public CoffeeController(
        ICoffeeService coffeeService,
        IWeatherService weatherService)
    {
        _coffeeService = coffeeService;
        _weatherService = weatherService;
    }

    [HttpGet("/brew-coffee")]
    public async Task<IActionResult> BrewCoffee()
    {
        if (_coffeeService.IsAprilFools())
            return StatusCode(418);

        var temp = await _weatherService.GetCurrentTemperatureAsync();

        var allowed = _coffeeService.TryBrew(HttpContext);

        if (!allowed)
            return StatusCode(503);

        var message = temp > 30
            ? "Your refreshing iced coffee is ready"
            : "Your piping hot coffee is ready";

        var response = new CoffeeResponse
        {
            Message = message,
            Prepared = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
        };

        return Ok(response);
    }
}