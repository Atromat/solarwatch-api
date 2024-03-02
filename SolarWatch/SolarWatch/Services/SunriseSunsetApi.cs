using System.Net;

namespace SolarWatch.Services;

public class SunriseSunsetApi : ISunsetSunriseDataProvider
{
    private readonly ILogger<OpenWeatherMapApi> _logger;

    public SunriseSunsetApi(ILogger<OpenWeatherMapApi> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetDataByLongitudeLatitude(double latitude, double longitude)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&formatted=0";

        var client = new HttpClient();

        _logger.LogInformation("Calling sunrise-sunset.org API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetDataByLongitudeLatitudeAndDate(double latitude, double longitude, int year, int month, int day)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&formatted=0&date={year}-{month}-{day}";

        var client = new HttpClient();

        _logger.LogInformation("Calling sunrise-sunset.org API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}