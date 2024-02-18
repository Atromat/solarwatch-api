using Microsoft.AspNetCore.Authorization;
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
            _logger.LogError(e, "Error getting SunriseSunset data");
            return NotFound("Error getting SunriseSunset data");
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
    
    [HttpPost("PostSunriseSunset"), Authorize(Roles="Admin")]
    public async Task<ActionResult<SunriseSunset>> PostSunriseSunset(string cityName, 
        int sunriseYear, int sunriseMonth, int sunriseDay, int sunriseHour, int sunriseMinute, int sunriseSecond,
        int sunsetYear, int sunsetMonth, int sunsetDay, int sunsetHour, int sunsetMinute, int sunsetSecond,
        int dayLength)
    {
        try
        {
            City? city = _cityRepository.GetByName(cityName);
            
            if (city != null)
            {
                var sunriseSunset = new SunriseSunset
                {
                    City = city,
                    Sunrise = new DateTime(sunriseYear, sunriseMonth, sunriseDay, sunriseHour, sunriseMinute, sunriseSecond),
                    Sunset = new DateTime(sunsetYear, sunsetMonth, sunsetDay, sunsetHour, sunsetMinute, sunsetSecond),
                    DayLength = dayLength
                };
                
                _sunriseSunsetRepository.Add(sunriseSunset);
                
                return Ok("Successfully added SunriseSunset.");
            }

            var cityFromApi = await GetCity(cityName);
            _cityRepository.Add(cityFromApi);
            
            var sunriseSunsetForDb = new SunriseSunset
            {
                City = cityFromApi,
                Sunrise = new DateTime(sunriseYear, sunriseMonth, sunriseDay, sunriseHour, sunriseMinute, sunriseSecond),
                Sunset = new DateTime(sunsetYear, sunsetMonth, sunsetDay, sunsetHour, sunsetMinute, sunsetSecond),
                DayLength = dayLength
            };
            
            _sunriseSunsetRepository.Add(sunriseSunsetForDb);
            
            return Ok("Successfully added SunriseSunset.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error posting SunriseSunset data");
            return StatusCode(500, "Error posting SunriseSunset data");
        }
    }
    
    [HttpPatch("UpdateSunriseSunset"), Authorize(Roles="Admin")]
    public async Task<ActionResult<SunriseSunset>> UpdateSunriseSunset(int id, string cityName, 
        int sunriseYear, int sunriseMonth, int sunriseDay, int sunriseHour, int sunriseMinute, int sunriseSecond,
        int sunsetYear, int sunsetMonth, int sunsetDay, int sunsetHour, int sunsetMinute, int sunsetsSecond,
        int dayLength)
    {
        try
        {
            City? city = _cityRepository.GetByName(cityName);

            SunriseSunset? sunriseSunsetFromDb = _sunriseSunsetRepository.GetById(id);

            if (sunriseSunsetFromDb == null)
            {
                return NotFound($"Couldn't update SunriseSunset by id {id} because it doesn't exist.");
            }
            
            if (city != null)
            {
                var sunriseSunset = new SunriseSunset
                {
                    Id = id,
                    City = city,
                    Sunrise = new DateTime(sunriseYear, sunriseMonth, sunriseDay, sunriseHour, sunriseMinute, sunriseSecond),
                    Sunset = new DateTime(sunsetYear, sunsetMonth, sunsetDay, sunsetHour, sunsetMinute, sunsetsSecond),
                    DayLength = dayLength
                };
                
                _sunriseSunsetRepository.Update(sunriseSunset);
                
                return Ok("Successfully updated SunriseSunset.");
            }

            var cityFromApi = await GetCity(cityName);
            _cityRepository.Add(cityFromApi);
            
            var sunriseSunsetForDb = new SunriseSunset
            {
                Id = id,
                City = cityFromApi,
                Sunrise = new DateTime(sunriseYear, sunriseMonth, sunriseDay, sunriseHour, sunriseMinute, sunriseSecond),
                Sunset = new DateTime(sunsetYear, sunsetMonth, sunsetDay, sunsetHour, sunsetMinute, sunsetsSecond),
                DayLength = dayLength
            };
            
            _sunriseSunsetRepository.Update(sunriseSunsetForDb);
            
            return Ok("Successfully updated SunriseSunset.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating SunriseSunset data");
            return StatusCode(500, "Error updating SunriseSunset data");
        }
    }
    
    [HttpDelete("DeleteSunriseSunset"), Authorize(Roles="Admin")]
    public async Task<ActionResult<SunriseSunset>> DeleteSunriseSunset(int id)
    {
        try
        {
            SunriseSunset? sunriseSunsetFromDb = _sunriseSunsetRepository.GetById(id);

            if (sunriseSunsetFromDb == null)
            {
                return NotFound($"Couldn't delete SunriseSunset by id {id} because it doesn't exist.");
            }
            
            _sunriseSunsetRepository.Delete(id);
            
            return Ok("Successfully deleted SunriseSunset.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting SunriseSunset data");
            return StatusCode(500, "Error deleting SunriseSunset data");
        }
    }
}