namespace SolarWatch.Services;

public interface ICoordDataProvider
{
    Task<string> GetDataByCity(string cityName);
}