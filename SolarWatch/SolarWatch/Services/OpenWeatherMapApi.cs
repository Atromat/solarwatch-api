using System.Net;

namespace SolarWatch.Services;

public class OpenWeatherMapApi : ICityDataProvider
{
    private readonly ILogger<OpenWeatherMapApi> _logger;
    
    public OpenWeatherMapApi(ILogger<OpenWeatherMapApi> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetDataByCity(string cityName)
    {
        var apiKey = "fa4b8e8127a50e1729797483e137687f";
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit={1}&appid={apiKey}";

        var client = new HttpClient();

        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}