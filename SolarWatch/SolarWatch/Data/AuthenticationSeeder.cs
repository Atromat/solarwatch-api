using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Data;

public class AuthenticationSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfigurationRoot _config;
    
    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();
    }
    
    public void AddRoles()
    {
        var tAdmin = CreateAdminRole();
        tAdmin.Wait();

        var tUser = CreateUserRole();
        tUser.Wait();
    }

    async Task CreateAdminRole()
    {
        await _roleManager.CreateAsync(new IdentityRole(_config["AdminRole"]));
    }

    async Task CreateUserRole()
    {
        await _roleManager.CreateAsync(new IdentityRole(_config["UserRole"]));
    }

    public void AddAdmin()
    {
        var tAdmin = CreateAdminIfNotExists();
        tAdmin.Wait();
    }

    async Task CreateAdminIfNotExists()
    {
        var adminInDb = await _userManager.FindByEmailAsync("admin@admin.hu");
        if (adminInDb == null)
        {
            var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.hu" };
            var adminCreated = await _userManager.CreateAsync(admin, _config["AdminPassword"]);

            if (adminCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, _config["AdminRole"]);
            }
        }
    }
}