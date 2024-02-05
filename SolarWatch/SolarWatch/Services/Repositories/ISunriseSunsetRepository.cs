using SolarWatch.Model;

namespace SolarWatch.Services.Repositories;

public interface ISunriseSunsetRepository
{
    IEnumerable<SunriseSunset> GetAll();
    SunriseSunset? GetByName(string cityName);
    SunriseSunset? GetByNameAndDate(string cityName, DateTime dateTime);

    void Add(SunriseSunset sunriseSunset);
    void Delete(SunriseSunset sunriseSunset);
    void Update(SunriseSunset sunriseSunset);
}