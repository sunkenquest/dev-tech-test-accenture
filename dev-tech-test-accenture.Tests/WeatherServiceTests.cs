using System.Net;
using System.Text;
using System.Text.Json;
using dev_tech_test_accenture.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace dev_tech_test_accenture.Tests;

public class WeatherServiceTests
{
    private static (WeatherService service, Mock<HttpMessageHandler> handlerMock, IConfiguration config)
        CreateService(string jsonResponse)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var configData = new Dictionary<string, string>
        {
            { "OpenWeather:BaseUrl", "https://api.openweathermap.org/data/2.5/" },
            { "OpenWeather:ApiKey", "test-key" },
            { "OpenWeather:City", "Manila" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var service = new WeatherService(httpClient, config);

        return (service, handlerMock, config);
    }

    [Fact]
    public async Task GetCurrentTemperatureAsync_ReturnsTemperature()
    {
        var json = """
        {
            "main": {
                "temp": 32.5
            }
        }
        """;

        var (service, handlerMock, _) = CreateService(json);

        var result = await service.GetCurrentTemperatureAsync();

        Assert.Equal(32.5, result);
    }

    [Fact]
    public async Task GetCurrentTemperatureAsync_CallsCorrectUrl()
    {
        var json = """{ "main": { "temp": 20 } }""";

        var (service, handlerMock, config) = CreateService(json);

        HttpRequestMessage? capturedRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        await service.GetCurrentTemperatureAsync();

        Assert.NotNull(capturedRequest);

        var expectedBaseUrl = config["OpenWeather:BaseUrl"];
        Assert.Contains("weather?q=Manila", capturedRequest!.RequestUri!.ToString());
        Assert.Contains("appid=test-key", capturedRequest.RequestUri.ToString());
    }

    [Theory]
    [InlineData(10.5)]
    [InlineData(25.0)]
    [InlineData(35.7)]
    [InlineData(0.0)]
    public async Task GetCurrentTemperatureAsync_ReturnsExpectedTemperature(double expectedTemp)
    {
        var json = $$"""
        {
            "main": {
                "temp": {{expectedTemp.ToString(System.Globalization.CultureInfo.InvariantCulture)}}
            }
        }
        """;

        var (service, _, _) = CreateService(json);

        var result = await service.GetCurrentTemperatureAsync();

        Assert.Equal(expectedTemp, result);
    }
}