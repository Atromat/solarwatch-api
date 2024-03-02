namespace SolarWatch.Services;

public interface ISunsetSunriseDataProvider
{
    Task<string> GetDataByLongitudeLatitude(double latitude, double longitude);
    Task<string> GetDataByLongitudeLatitudeAndDate(double latitude, double longitude, int year, int month, int day);
}