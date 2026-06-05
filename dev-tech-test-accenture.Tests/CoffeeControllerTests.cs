using dev_tech_test_accenture.Controllers;
using dev_tech_test_accenture.Models;
using dev_tech_test_accenture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dev_tech_test_accenture.Tests;

public class CoffeeControllerTests
{
    [Fact]
    public void BrewCoffee_ReturnsOk_WhenCoffeeCanBeBrewed()
    {
        var service = new FakeCoffeeService
        {
            TryBrewResult = true,
            IsAprilFoolsResult = false
        };

        var controller = new CoffeeController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.BrewCoffee();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(okResult.Value);

        Assert.Equal("Your piping hot coffee is ready", response.Message);
        Assert.False(string.IsNullOrWhiteSpace(response.Prepared));
    }

    [Fact]
    public void BrewCoffee_Returns503_WhenCoffeeCannotBeBrewed()
    {
        var service = new FakeCoffeeService
        {
            TryBrewResult = false,
            IsAprilFoolsResult = false
        };

        var controller = new CoffeeController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.BrewCoffee();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(503, statusResult.StatusCode);
    }

    [Fact]
    public void BrewCoffee_Returns418_OnAprilFools()
    {
        var service = new FakeCoffeeService
        {
            TryBrewResult = true,
            IsAprilFoolsResult = true
        };

        var controller = new CoffeeController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.BrewCoffee();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusResult.StatusCode);
    }
}

public class FakeCoffeeService : ICoffeeService
{
    public bool TryBrewResult { get; set; }

    public bool IsAprilFoolsResult { get; set; }

    public bool TryBrew(HttpContext context)
    {
        return TryBrewResult;
    }

    public bool IsAprilFools()
    {
        return IsAprilFoolsResult;
    }
}