using SolarWatch.Services;
using SolarWatch.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICoordDataProvider, OpenWeatherMapApi>();
builder.Services.AddSingleton<IWeatherMapJsonProcessor, WeatherMapWeatherMapJsonProcessor>();
builder.Services.AddSingleton<ISunsetSunriseDataProvider, SunriseSunsetApi>();
builder.Services.AddSingleton<ISunsetSunriseJsonProcessor, SunsetSunriseJsonProcessor>();
builder.Services.AddSingleton<ICityRepository, CityRepository>();
builder.Services.AddSingleton<ISunriseSunsetRepository, SunriseSunsetRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();