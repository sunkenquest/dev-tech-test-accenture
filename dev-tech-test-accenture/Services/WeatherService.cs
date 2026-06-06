using System.Text.Json;

namespace dev_tech_test_accenture.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<double> GetCurrentTemperatureAsync()
    {
        var baseUrl = _config["OpenWeather:BaseUrl"];
        var apiKey = _config["OpenWeather:ApiKey"];
        var city = _config["OpenWeather:City"];

        var url = $"{baseUrl}weather?q={city}&appid={apiKey}&units=metric";

        var response = await _httpClient.GetStringAsync(url);

        using var json = JsonDocument.Parse(response);

        return json.RootElement
            .GetProperty("main")
            .GetProperty("temp")
            .GetDouble();
    }
}