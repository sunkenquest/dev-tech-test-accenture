namespace dev_tech_test_accenture.Services;

public interface IWeatherService
{
    Task<double> GetCurrentTemperatureAsync();
}