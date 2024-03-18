using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Data;
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
    private UnitOfWork _unitOfWork;
    
    public SolarWatchController(
        ILogger<SolarWatchController> logger, 
        IWeatherMapJsonProcessor weatherMapJsonProcessor,
        ICityDataProvider cityDataProvider, 
        ISunsetSunriseDataProvider sunsetSunriseDataProvider,
        ISunsetSunriseJsonProcessor sunsetSunriseJsonProcessor,
        SolarWatchContext solarWatchContext)
    {
        _logger = logger;
        _weatherMapJsonProcessor = weatherMapJsonProcessor;
        _cityDataProvider = cityDataProvider;
        _sunsetSunriseDataProvider = sunsetSunriseDataProvider;
        _sunsetSunriseJsonProcessor = sunsetSunriseJsonProcessor;
        _unitOfWork = new UnitOfWork(solarWatchContext);
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
            City? city = _unitOfWork.CityRepository.GetByName(cityName);
            
            if (city != null)
            {
                SunriseSunset? sunriseSunset = _unitOfWork.SunriseSunsetRepository.GetByNameAndDate(cityName, new DateTime(year, month, day));
                
                if (sunriseSunset == null)
                {
                    var sunriseSunsetFromApi = await GetSunriseSunsetByCityAndDate(city, year, month, day);
                    _unitOfWork.SunriseSunsetRepository.Add(sunriseSunsetFromApi);
                    _unitOfWork.Save();
                    return Ok(sunriseSunsetFromApi);
                }
                
                return Ok(sunriseSunset);
            }

            var cityFromApi = await GetCity(cityName);
            _unitOfWork.CityRepository.Add(cityFromApi);
            _unitOfWork.Save();
            var sunriseSunsetFromBothApi = await GetSunriseSunset(cityFromApi);
            _unitOfWork.SunriseSunsetRepository.Add(sunriseSunsetFromBothApi);
            _unitOfWork.Save();
            
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
    
    private async Task<SunriseSunset> GetSunriseSunsetByCityAndDate(City city, int year, int month, int day)
    {
        var sunriseSunsetData = 
            await _sunsetSunriseDataProvider.GetDataByLongitudeLatitudeAndDate(city.Latitude, city.Longitude, year, month, day);
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
    public async Task<ActionResult<SunriseSunset>> PostSunriseSunset(
        string cityName, 
        int year, int month, int day, 
        int sunriseHour, int sunriseMinute, int sunriseSecond,
        int sunsetHour, int sunsetMinute, int sunsetSecond,
        int dayLength)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(cityName);
            
            if (city != null)
            {
                var sunriseSunset = new SunriseSunset
                {
                    City = city,
                    Sunrise = new DateTime(year, month, day, sunriseHour, sunriseMinute, sunriseSecond),
                    Sunset = new DateTime(year, month, day, sunsetHour, sunsetMinute, sunsetSecond),
                    DayLength = dayLength
                };
                
                _unitOfWork.SunriseSunsetRepository.Add(sunriseSunset);
                _unitOfWork.Save();
                
                return Ok("Successfully added SunriseSunset.");
            }

            var cityFromApi = await GetCity(cityName);
            _unitOfWork.CityRepository.Add(cityFromApi);
            
            var sunriseSunsetForDb = new SunriseSunset
            {
                City = cityFromApi,
                Sunrise = new DateTime(year, month, day, sunriseHour, sunriseMinute, sunriseSecond),
                Sunset = new DateTime(year, month, day, sunsetHour, sunsetMinute, sunsetSecond),
                DayLength = dayLength
            };
            
            _unitOfWork.SunriseSunsetRepository.Add(sunriseSunsetForDb);
            _unitOfWork.Save();
            
            return Ok("Successfully added SunriseSunset.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error posting SunriseSunset data");
            return StatusCode(500, "Error posting SunriseSunset data");
        }
    }
    
    [HttpPatch("UpdateSunriseSunset"), Authorize(Roles="Admin")]
    public async Task<ActionResult<SunriseSunset>> UpdateSunriseSunset(
        int id, string cityName, 
        int year, int month, int day, 
        int sunriseHour, int sunriseMinute, int sunriseSecond, 
        int sunsetHour, int sunsetMinute, int sunsetsSecond,
        int dayLength)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(cityName);

            SunriseSunset? sunriseSunsetFromDb = _unitOfWork.SunriseSunsetRepository.GetById(id);

            if (sunriseSunsetFromDb == null)
            {
                return NotFound($"Couldn't update SunriseSunset by id {id} because it doesn't exist.");
            }
            
            if (city != null)
            {
                sunriseSunsetFromDb.City = city;
                sunriseSunsetFromDb.Sunrise = new DateTime(year, month, day, sunriseHour, sunriseMinute, sunriseSecond);
                sunriseSunsetFromDb.Sunset = new DateTime(year, month, day, sunsetHour, sunsetMinute, sunsetsSecond);
                sunriseSunsetFromDb.DayLength = dayLength;
                
                _unitOfWork.SunriseSunsetRepository.Update(sunriseSunsetFromDb);
                _unitOfWork.Save();
                
                return Ok("Successfully updated SunriseSunset.");
            }

            var cityFromApi = await GetCity(cityName);
            _unitOfWork.CityRepository.Add(cityFromApi);
            
            sunriseSunsetFromDb.City = city;
            sunriseSunsetFromDb.Sunrise = new DateTime(year, month, day, sunriseHour, sunriseMinute, sunriseSecond);
            sunriseSunsetFromDb.Sunset = new DateTime(year, month, day, sunsetHour, sunsetMinute, sunsetsSecond);
            sunriseSunsetFromDb.DayLength = dayLength;
            
            _unitOfWork.SunriseSunsetRepository.Update(sunriseSunsetFromDb);
            _unitOfWork.Save();
            
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
            SunriseSunset? sunriseSunsetFromDb = _unitOfWork.SunriseSunsetRepository.GetById(id);

            if (sunriseSunsetFromDb == null)
            {
                return NotFound($"Couldn't delete SunriseSunset by id {id} because it doesn't exist.");
            }
            
            _unitOfWork.SunriseSunsetRepository.Delete(id);
            _unitOfWork.Save();
            
            return Ok("Successfully deleted SunriseSunset.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting SunriseSunset data");
            return StatusCode(500, "Error deleting SunriseSunset data");
        }
    }
}