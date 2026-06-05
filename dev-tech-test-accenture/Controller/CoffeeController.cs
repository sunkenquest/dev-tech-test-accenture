using Microsoft.AspNetCore.Mvc;
using dev_tech_test_accenture.Models;

namespace dev_tech_test_accenture.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    [HttpGet("/brew-coffee")]
    public IActionResult BrewCoffee()
    {
        var response = new CoffeeResponse
        {
            Message = "Your piping hot coffee is ready",
            Prepared = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
        };

        return Ok(response);
    }
}