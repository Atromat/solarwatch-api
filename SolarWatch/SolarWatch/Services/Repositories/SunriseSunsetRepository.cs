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
    
    public SunriseSunset? GetByNameAndDate(string cityName, DateOnly dateOnly)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunriseSunsets.FirstOrDefault(
            s => s.City.Name == cityName &&
                 DateOnly.FromDateTime(s.Sunrise) == dateOnly);
    }

    public void Add(SunriseSunset sunriseSunset)
    {
        using var dbContext = new SolarWatchContext();
        dbContext.Add(sunriseSunset);
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