namespace SolarWatch.Services;

public interface ICoordDataProvider
{
    string GetDataByCity(string cityName);
}