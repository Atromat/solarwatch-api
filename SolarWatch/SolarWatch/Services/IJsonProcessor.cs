namespace SolarWatch.Services;

public interface IJsonProcessor
{
    (double, double) GetLongitudeLatitude(string openWeatherMapApiAnswer);
}