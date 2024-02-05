using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.Services;

public class WeatherMapWeatherMapJsonProcessor : IWeatherMapJsonProcessor
{
    public (double, double) GetLongitudeLatitude(string openWeatherMapApiAnswer)
    {
        JsonDocument json = JsonDocument.Parse(openWeatherMapApiAnswer);
        var firstJsonElement = json.RootElement.EnumerateArray().ToArray().First();
        var latitude = firstJsonElement.GetProperty("lat").GetDouble();
        var longitude = firstJsonElement.GetProperty("lon").GetDouble();

        return (latitude, longitude);
    }

    public City GetCity(string openWeatherMapApiAnswer)
    {
        JsonDocument json = JsonDocument.Parse(openWeatherMapApiAnswer);
        var firstJsonElement = json.RootElement.EnumerateArray().ToArray().First();
        var name = firstJsonElement.GetProperty("name").ToString();
        var country = firstJsonElement.GetProperty("country").ToString();
        var latitude = firstJsonElement.GetProperty("lat").GetDouble();
        var longitude = firstJsonElement.GetProperty("lon").GetDouble();

        return new City{Name = name, Latitude = latitude, Longitude = longitude, Country = country};
    }
}