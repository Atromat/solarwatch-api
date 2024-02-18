using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Services;
using SolarWatch.Services.Repositories;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController : ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IWeatherMapJsonProcessor _weatherMapJsonProcessor;
    private readonly ICityDataProvider _cityDataProvider;
    private readonly ISunsetSunriseDataProvider _sunsetSunriseDataProvider;
    private readonly ISunsetSunriseJsonProcessor _sunsetSunriseJsonProcessor;
    private readonly ICityRepository _cityRepository;
    private readonly ISunriseSunsetRepository _sunriseSunsetRepository;

    public SolarWatchController(
        ILogger<SolarWatchController> logger, 
        IWeatherMapJsonProcessor weatherMapJsonProcessor,
        ICityDataProvider cityDataProvider, 
        ISunsetSunriseDataProvider sunsetSunriseDataProvider,
        ISunsetSunriseJsonProcessor sunsetSunriseJsonProcessor,
        ICityRepository cityRepository,
        ISunriseSunsetRepository sunriseSunsetRepository)
    {
        _logger = logger;
        _weatherMapJsonProcessor = weatherMapJsonProcessor;
        _cityDataProvider = cityDataProvider;
        _sunsetSunriseDataProvider = sunsetSunriseDataProvider;
        _sunsetSunriseJsonProcessor = sunsetSunriseJsonProcessor;
        _cityRepository = cityRepository;
        _sunriseSunsetRepository = sunriseSunsetRepository;
    }

    [HttpGet("GetSunriseTime")]
    public async Task<ActionResult<TimeOnly>> GetSunriseTime(string cityName)
    {
        try
        {
            var cityData = await _cityDataProvider.GetDataByCity(cityName);
            var cityLatLon = _weatherMapJsonProcessor.GetLongitudeLatitude(cityData);
            var sunriseData = await _sunsetSunriseDataProvider.GetDataByLongitudeLatitude(cityLatLon.Item1, cityLatLon.Item2);
            var sunriseTime = _sunsetSunriseJsonProcessor.GetSunrise(sunriseData);
            return Ok(sunriseTime);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sun rise data");
            return NotFound("Error getting sun rise data");
        }
    }
    
    [HttpGet("GetSunsetTime")]
    public async Task<ActionResult<TimeOnly>> GetSunsetTime(string cityName)
    {
        try
        {
            var cityData = await _cityDataProvider.GetDataByCity(cityName);
            var cityLatLon = _weatherMapJsonProcessor.GetLongitudeLatitude(cityData);
            var sunsetData = await _sunsetSunriseDataProvider.GetDataByLongitudeLatitude(cityLatLon.Item1, cityLatLon.Item2);
            var sunsetTime = _sunsetSunriseJsonProcessor.GetSunset(sunsetData);
            return Ok(sunsetTime);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sun rise data");
            return NotFound("Error getting sun rise data");
        }
    }
    
    [HttpGet("GetSunriseSunset"), Authorize(Roles="User, Admin")]
    public async Task<ActionResult<SunriseSunset>> GetSunriseSunset(string cityName, int year, int month, int day)
    {
        try
        {
            City? city = _cityRepository.GetByName(cityName);
            
            if (city != null)
            {
                SunriseSunset? sunriseSunset = _sunriseSunsetRepository.GetByNameAndDate(cityName, new DateTime(year, month, day));
                
                if (sunriseSunset == null)
                {
                    var sunriseSunsetFromApi = await GetSunriseSunset(city);
                    _sunriseSunsetRepository.Add(sunriseSunsetFromApi);
                    return Ok(sunriseSunsetFromApi);
                }
                
                return Ok(sunriseSunset);
            }

            var cityFromApi = await GetCity(cityName);
            _cityRepository.Add(cityFromApi);
            var sunriseSunsetFromBothApi = await GetSunriseSunset(cityFromApi);
            _sunriseSunsetRepository.Add(sunriseSunsetFromBothApi);
            //var sunriseSunsetFromApi = new SunriseSunset(); //hiba lenne miért??
            
            return Ok(sunriseSunsetFromBothApi);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sunrise/sunset data");
            return NotFound("Error getting sunrise/sunset data");
        }
    }

    private async Task<SunriseSunset> GetSunriseSunset(City city)
    {
        var sunriseSunsetData = await _sunsetSunriseDataProvider.GetDataByLongitudeLatitude(city.Latitude, city.Longitude);
        var sunriseDateTime = _sunsetSunriseJsonProcessor.GetSunriseDateTime(sunriseSunsetData);
        var sunsetDateTime = _sunsetSunriseJsonProcessor.GetSunsetDateTime(sunriseSunsetData);
        var dayLength = _sunsetSunriseJsonProcessor.GetDayLength(sunriseSunsetData);
        
        return new SunriseSunset{City = city, Sunrise = sunriseDateTime, Sunset = sunsetDateTime, DayLength = dayLength};
    }
    
    private async Task<SunriseSunset> GetSunriseSunset(string cityName)
    {
        var cityData = await _cityDataProvider.GetDataByCity(cityName);
        var city = _weatherMapJsonProcessor.GetCity(cityData);
        
        return await GetSunriseSunset(city);
    }

    private async Task<City> GetCity(string cityName)
    {
        var cityData = await _cityDataProvider.GetDataByCity(cityName);
        return _weatherMapJsonProcessor.GetCity(cityData);
    }
}