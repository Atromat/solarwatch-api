namespace SolarWatch.Services;

public interface ISunsetSunriseJsonProcessor
{
    TimeOnly GetSunrise(string sunsetSunriseData);
    TimeOnly GetSunset(string sunsetSunriseData);

    public DateTime GetSunriseDateTime(string sunsetSunriseData);

    public DateTime GetSunsetDateTime(string sunsetSunriseData);

    public int GetDayLength(string sunriseSunsetData);
}