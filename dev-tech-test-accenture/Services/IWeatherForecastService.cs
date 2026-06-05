using dev_tech_test_accenture.Models;

namespace dev_tech_test_accenture.Services;

public interface IWeatherForecastService
{
    IEnumerable<WeatherForecast> GetForecast();
}