using System.Net;

namespace SolarWatch.Services;

public class SunriseSunsetApi : ISunsetSunriseDataProvider
{
    private readonly ILogger<OpenWeatherMapApi> _logger;

    public SunriseSunsetApi(ILogger<OpenWeatherMapApi> logger)
    {
        _logger = logger;
    }

    public string GetDataByLongitudeLatitude(double latitude, double longitude)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}";

        var client = new WebClient();

        _logger.LogInformation("Calling sunrise-sunset.org API with url: {url}", url);
        return client.DownloadString(url);
    }
}