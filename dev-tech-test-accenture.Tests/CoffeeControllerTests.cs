using dev_tech_test_accenture.Controllers;
using dev_tech_test_accenture.Models;
using dev_tech_test_accenture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace dev_tech_test_accenture.Tests;

public class CoffeeControllerTests
{
    private CoffeeController CreateController(
        Mock<ICoffeeService> coffeeMock,
        Mock<IWeatherService> weatherMock)
    {
        return new CoffeeController(coffeeMock.Object, weatherMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task BrewCoffee_ReturnsHotCoffee_WhenColdWeather()
    {
        var coffeeMock = new Mock<ICoffeeService>();
        coffeeMock.Setup(x => x.IsAprilFools()).Returns(false);
        coffeeMock.Setup(x => x.TryBrew(It.IsAny<HttpContext>())).Returns(true);

        var weatherMock = new Mock<IWeatherService>();
        weatherMock.Setup(x => x.GetCurrentTemperatureAsync())
                    .ReturnsAsync(25);

        var controller = CreateController(coffeeMock, weatherMock);

        var result = await controller.BrewCoffee();

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(ok.Value);

        Assert.Equal("Your piping hot coffee is ready", response.Message);
    }

    [Fact]
    public async Task BrewCoffee_ReturnsIcedCoffee_WhenHotWeather()
    {
        var coffeeMock = new Mock<ICoffeeService>();
        coffeeMock.Setup(x => x.IsAprilFools()).Returns(false);
        coffeeMock.Setup(x => x.TryBrew(It.IsAny<HttpContext>())).Returns(true);

        var weatherMock = new Mock<IWeatherService>();
        weatherMock.Setup(x => x.GetCurrentTemperatureAsync())
                    .ReturnsAsync(35);

        var controller = CreateController(coffeeMock, weatherMock);

        var result = await controller.BrewCoffee();

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(ok.Value);

        Assert.Equal("Your refreshing iced coffee is ready", response.Message);
    }

    [Fact]
    public async Task BrewCoffee_Returns503_WhenCoffeeNotAllowed()
    {
        var coffeeMock = new Mock<ICoffeeService>();
        coffeeMock.Setup(x => x.IsAprilFools()).Returns(false);
        coffeeMock.Setup(x => x.TryBrew(It.IsAny<HttpContext>())).Returns(false);

        var weatherMock = new Mock<IWeatherService>();
        weatherMock.Setup(x => x.GetCurrentTemperatureAsync())
                    .ReturnsAsync(25);

        var controller = CreateController(coffeeMock, weatherMock);

        var result = await controller.BrewCoffee();

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(503, status.StatusCode);
    }

    [Fact]
    public async Task BrewCoffee_Returns418_OnAprilFools()
    {
        var coffeeMock = new Mock<ICoffeeService>();
        coffeeMock.Setup(x => x.IsAprilFools()).Returns(true);

        var weatherMock = new Mock<IWeatherService>();

        var controller = CreateController(coffeeMock, weatherMock);

        var result = await controller.BrewCoffee();

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, status.StatusCode);
    }
}