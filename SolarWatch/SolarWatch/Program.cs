using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarWatch.Services;
using SolarWatch.Services.Repositories;
using SolarWatch.Data;
using SolarWatch.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AddServices();
ConfigureSwagger();
AddDbContext();
AddIdentity();
AddAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
        policy  =>
        {
            policy
                //.WithOrigins("*") //doesn't work with credentiels included
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrWhiteSpace(origin)) return false;
                    // Only add this to allow testing with localhost, remove this line in production!
                    if (origin.ToLower().StartsWith("http://localhost")) return true;
                    // Insert your production domain here.
                    if (origin.ToLower().StartsWith("https://dev.mydomain.com")) return true;
                    return false;
                });
        });
});

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();

authenticationSeeder.AddRoles();

authenticationSeeder.AddAdmin();

app.Run();

void AddServices()
{
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddSingleton<ICityDataProvider, OpenWeatherMapApi>();
    builder.Services.AddSingleton<IWeatherMapJsonProcessor, WeatherMapWeatherMapJsonProcessor>();
    builder.Services.AddSingleton<ISunsetSunriseDataProvider, SunriseSunsetApi>();
    builder.Services.AddSingleton<ISunsetSunriseJsonProcessor, SunsetSunriseJsonProcessor>();
    builder.Services.AddSingleton<ICityRepository, CityRepository>();
    builder.Services.AddSingleton<ISunriseSunsetRepository, SunriseSunsetRepository>();
    builder.Services.AddScoped<AuthenticationSeeder>();
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}

void AddDbContext()
{
    builder.Services.AddDbContext<UsersContext>();
}

void AddAuthentication()
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var validIssuer = builder.Configuration["ValidIssuer"];
            var validAudience = builder.Configuration["ValidAudience"];
            var issuerSigningKey = builder.Configuration["IssuerSigningKey"];
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuerSigningKey)
                ),
            };
            
            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["token"];
                    return Task.CompletedTask;
                }
            };
        });
    
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>() //Enable Identity roles 
        .AddEntityFrameworkStores<UsersContext>();
}

public partial class Program { }