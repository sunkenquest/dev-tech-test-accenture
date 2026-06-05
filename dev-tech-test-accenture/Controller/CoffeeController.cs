using Microsoft.AspNetCore.Mvc;
using dev_tech_test_accenture.Models;
using dev_tech_test_accenture.Services;

namespace dev_tech_test_accenture.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly ICoffeeService _coffeeService;

    public CoffeeController(ICoffeeService coffeeService)
    {
        _coffeeService = coffeeService;
    }

    [HttpGet("/brew-coffee")]
    public IActionResult BrewCoffee()
    {
        var allowed = _coffeeService.TryBrew(HttpContext);

        if (!allowed)
        {
            return StatusCode(503);
        }

        if (_coffeeService.IsAprilFools())
            return StatusCode(418);
            

        var response = new CoffeeResponse
        {
            Message = "Your piping hot coffee is ready",
            Prepared = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
        };

        return Ok(response);
    }
}