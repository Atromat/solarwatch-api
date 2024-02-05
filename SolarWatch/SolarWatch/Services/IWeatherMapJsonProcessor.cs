using SolarWatch.Model;

namespace SolarWatch.Services;

public interface IWeatherMapJsonProcessor
{
    (double, double) GetLongitudeLatitude(string openWeatherMapApiAnswer);
    City GetCity(string openWeatherMapApiAnswer);
}