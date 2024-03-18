using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repositories;

public class SunriseSunsetRepository : ISunriseSunsetRepository
{
    private SolarWatchContext dbContext;

    public SunriseSunsetRepository(SolarWatchContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<SunriseSunset> GetAll()
    {
        return dbContext.SunriseSunsets.ToList();
    }

    public SunriseSunset? GetById(int id)
    {
        return dbContext.SunriseSunsets.FirstOrDefault(s => s.Id == id);
    }
    
    public SunriseSunset? GetByName(string cityName)
    {
        return dbContext.SunriseSunsets.FirstOrDefault(s => s.City.Name == cityName);
    }
    
    public SunriseSunset? GetByNameAndDate(string cityName, DateTime dateTime)
    {
        return dbContext.SunriseSunsets
            .Include(sunriseSunset => sunriseSunset.City)
            .FirstOrDefault(
            s => s.City.Name == cityName &&
                 s.Sunrise.Year == dateTime.Year && s.Sunrise.Month == dateTime.Month && s.Sunrise.Day == dateTime.Day);
    }

    public void Add(SunriseSunset sunriseSunset)
    {
        var cityDb = dbContext.Cities.FirstOrDefault(city => city.Id == sunriseSunset.City.Id);
        dbContext.Add(new SunriseSunset
        {
            City = cityDb, 
            Sunrise = sunriseSunset.Sunrise, 
            Sunset = sunriseSunset.Sunset, 
            DayLength = sunriseSunset.DayLength
        });
    }

    public void Delete(int id)
    {
        var sunriseSunset = dbContext.SunriseSunsets.First(s => s.Id == id);
        dbContext.Remove(sunriseSunset);
    }

    public void Update(SunriseSunset sunriseSunset)
    {  
        dbContext.Update(sunriseSunset);
    }
}