namespace SolarWatch.Services;

public interface ICityDataProvider
{
    Task<string> GetDataByCity(string cityName);
}