using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data;

public class SolarWatchContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunriseSunsets { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)                     //for getting it from appsettings.json
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  //for getting it from appsettings.json
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();
        
        // optionsBuilder.UseSqlServer(
        //     configuration.GetConnectionString("SolarWatchDBLocalConnection"));  //for getting it from appsettings.json
        
        optionsBuilder.UseSqlServer(configuration["ConnString"]);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //Configure the City entity - making the 'Name' unique
        builder.Entity<City>()
            .HasIndex(u => u.Name)
            .IsUnique();
    
        builder.Entity<City>()
            .HasData(
                new City { Id = 1, Name = "London", Latitude = 51.509865, Longitude = -0.118092, State = null, Country = "England"},
                new City { Id = 2, Name = "Budapest", Latitude = 47.497913, Longitude = 19.040236, State = null, Country = "Hungary" },
                new City { Id = 3, Name = "Paris", Latitude = 48.864716, Longitude = 2.349014, State = null, Country = "France"}
            );
    }
}