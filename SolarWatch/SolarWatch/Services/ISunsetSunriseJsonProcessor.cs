namespace SolarWatch.Services;

public interface ISunsetSunriseJsonProcessor
{
    TimeOnly GetSunrise(string sunsetSunriseData);
    TimeOnly GetSunset(string sunsetSunriseData);
}