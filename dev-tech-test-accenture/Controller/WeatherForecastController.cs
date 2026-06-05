using dev_tech_test_accenture.Services;
using Microsoft.AspNetCore.Mvc;

namespace dev_tech_test_accenture.Controller;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(IWeatherForecastService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(service.GetForecast());
    }
}