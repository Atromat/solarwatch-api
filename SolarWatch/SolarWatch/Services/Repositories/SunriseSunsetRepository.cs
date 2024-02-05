using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repositories;

public class SunriseSunsetRepository : ISunriseSunsetRepository
{
    public IEnumerable<SunriseSunset> GetAll()
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.ToList();
    }

    public SunriseSunset? GetByName(string cityName)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.FirstOrDefault(c => c.City.Name == cityName);
    }
    
    public SunriseSunset? GetByNameAndDate(string cityName, DateTime dateTime)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.FirstOrDefault(
            s => s.City.Name == cityName &&
                 s.Sunrise == dateTime);
    }

    public void Add(SunriseSunset sunriseSunset)
    {
        using var dbContext = new SolarWatchContext();
        //dbContext.Add(sunriseSunset);
        var cityDb = dbContext.Cities.FirstOrDefault(city => city.Id == sunriseSunset.City.Id);
        dbContext.Add(new SunriseSunset
        {
            City = cityDb, 
            Sunrise = sunriseSunset.Sunrise, 
            Sunset = sunriseSunset.Sunset, 
            DayLength = sunriseSunset.DayLength
        });
        dbContext.SaveChanges();
    }

    public void Delete(SunriseSunset sunriseSunset)
    {
        using var dbContext = new SolarWatchContext();
        dbContext.Remove(sunriseSunset);
        dbContext.SaveChanges();
    }

    public void Update(SunriseSunset sunriseSunset)
    {  
        using var dbContext = new SolarWatchContext();
        dbContext.Update(sunriseSunset);
        dbContext.SaveChanges();
    }
}