using SolarWatch.Model;

namespace SolarWatch.Services.Repositories;

public interface ISunriseSunsetRepository
{
    IEnumerable<SunriseSunset> GetAll();
    public SunriseSunset? GetById(int id);
    SunriseSunset? GetByName(string cityName);
    SunriseSunset? GetByNameAndDate(string cityName, DateTime dateTime);

    void Add(SunriseSunset sunriseSunset);
    void Delete(int id);
    void Update(SunriseSunset sunriseSunset);
}