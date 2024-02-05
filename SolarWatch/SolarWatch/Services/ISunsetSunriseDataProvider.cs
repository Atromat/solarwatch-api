namespace SolarWatch.Services;

public interface ISunsetSunriseDataProvider
{
    Task<string> GetDataByLongitudeLatitude(double latitude, double longitude);
}