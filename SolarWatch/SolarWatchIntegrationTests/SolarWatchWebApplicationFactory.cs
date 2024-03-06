using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolarWatch.Data;

namespace SolarWatchIntegrationTests;

public class SolarWatchWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<SolarWatchContext>));

            services.Remove(dbContextDescriptor);

            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SolarWatchContext>();
            var authSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            authSeeder.AddRoles();
            authSeeder.AddAdmin();
        });
    }
}