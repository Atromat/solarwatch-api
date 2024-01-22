using System.Net;

namespace SolarWatch.Services;

public class OpenWeatherMapApi : ICoordDataProvider
{
    private readonly ILogger<OpenWeatherMapApi> _logger;
    
    public OpenWeatherMapApi(ILogger<OpenWeatherMapApi> logger)
    {
        _logger = logger;
    }

    public string GetDataByCity(string cityName)
    {
        var apiKey = "fa4b8e8127a50e1729797483e137687f";
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit={1}&appid={apiKey}";

        var client = new WebClient();

        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        return client.DownloadString(url);
    }
}