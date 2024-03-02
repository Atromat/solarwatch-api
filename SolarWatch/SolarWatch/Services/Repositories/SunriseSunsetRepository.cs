using Microsoft.EntityFrameworkCore;
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

    public SunriseSunset? GetById(int id)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.FirstOrDefault(s => s.Id == id);
    }
    
    public SunriseSunset? GetByName(string cityName)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.FirstOrDefault(s => s.City.Name == cityName);
    }
    
    public SunriseSunset? GetByNameAndDate(string cityName, DateTime dateTime)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets
            .Include(sunriseSunset => sunriseSunset.City)
            .FirstOrDefault(
            s => s.City.Name == cityName &&
                 s.Sunrise.Year == dateTime.Year && s.Sunrise.Month == dateTime.Month && s.Sunrise.Day == dateTime.Day);
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

    public void Delete(int id)
    {
        using var dbContext = new SolarWatchContext();
        var sunriseSunset = dbContext.SunriseSunsets.First(s => s.Id == id);
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