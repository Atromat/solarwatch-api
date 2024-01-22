namespace SolarWatch.Services;

public interface ISunsetSunriseDataProvider
{
    string GetDataByLongitudeLatitude(double latitude, double longitude);
}