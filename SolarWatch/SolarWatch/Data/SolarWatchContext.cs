using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data;

public class SolarWatchContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunriseSunsets { get; set; }

    public SolarWatchContext()
    {
    }

    public SolarWatchContext(DbContextOptions options) : base(options)
    {
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
        
        base.OnModelCreating(builder);
    }
}