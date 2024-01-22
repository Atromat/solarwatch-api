using System.Text.Json;

namespace SolarWatch.Services;

public class JsonProcessor : IJsonProcessor
{
    public (double, double) GetLongitudeLatitude(string openWeatherMapApiAnswer)
    {
        JsonDocument json = JsonDocument.Parse(openWeatherMapApiAnswer);
        var firstJsonElement = json.RootElement.EnumerateArray().ToArray().First();
        var latitude = firstJsonElement.GetProperty("lat").GetDouble();
        var longitude = firstJsonElement.GetProperty("lon").GetDouble();

        return (latitude, longitude);
    }
}