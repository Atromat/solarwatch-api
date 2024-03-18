using SolarWatch.Data;

namespace SolarWatch.Services.Repositories;

public class UnitOfWork : IDisposable
{
    private SolarWatchContext context;
    private ICityRepository _cityRepository;
    private ISunriseSunsetRepository _sunriseSunsetRepository;

    public UnitOfWork(SolarWatchContext solarWatchContext)
    {
        context = solarWatchContext;
    }

    public ICityRepository CityRepository
    {
        get
        {

            if (this._cityRepository == null)
            {
                this._cityRepository = new CityRepository(context);
            }
            return _cityRepository;
        }
    }

    public ISunriseSunsetRepository SunriseSunsetRepository
    {
        get
        {

            if (this._sunriseSunsetRepository == null)
            {
                this._sunriseSunsetRepository = new SunriseSunsetRepository(context);
            }
            return _sunriseSunsetRepository;
        }
    }

    public void Save()
    {
        context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}