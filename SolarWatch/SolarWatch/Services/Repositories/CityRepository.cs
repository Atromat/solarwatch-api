using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repositories;

public class CityRepository : ICityRepository
{
    private SolarWatchContext dbContext;

    public CityRepository(SolarWatchContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<City> GetAll()
    {
        return dbContext.Cities.ToList();
    }

    public City? GetByName(string name)
    {
        return dbContext.Cities.FirstOrDefault(c => c.Name == name);
    }

    public void Add(City city)
    {
        dbContext.Add(city);
    }

    public void Delete(City city)
    {
        dbContext.Remove(city);
    }

    public void Update(City city)
    {  
        dbContext.Update(city);
    }
}