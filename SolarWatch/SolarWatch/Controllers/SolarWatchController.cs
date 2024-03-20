using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<IdentityUser> _userManager;
    
    public SolarWatchController(
        ILogger<SolarWatchController> logger, 
        IWeatherMapJsonProcessor weatherMapJsonProcessor,
        ICityDataProvider cityDataProvider, 
        ISunsetSunriseDataProvider sunsetSunriseDataProvider,
        ISunsetSunriseJsonProcessor sunsetSunriseJsonProcessor,
        SolarWatchContext solarWatchContext,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _weatherMapJsonProcessor = weatherMapJsonProcessor;
        _cityDataProvider = cityDataProvider;
        _sunsetSunriseDataProvider = sunsetSunriseDataProvider;
        _sunsetSunriseJsonProcessor = sunsetSunriseJsonProcessor;
        _unitOfWork = new UnitOfWork(solarWatchContext);
        _userManager = userManager;
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
    
    [HttpGet("GetRole")]
    public async Task<ActionResult<string>> GetRole()
    {
        try
        {
            if (User.Identity == null)
            {
                return Ok("guest");
            }
            
            if (User.Identity.Name == null)
            {
                return Ok("guest");
            }
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.GetRolesAsync(user);

            return Ok(role.First());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Couldn't get role.");
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
    
    [HttpGet("GetCityFromDb"), Authorize(Roles="Admin")]
    public async Task<ActionResult<City>> GetCityFromDb(string name)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(name);
            
            if (city == null)
            {
                return BadRequest("A city with that name doesn't exists.");
            }
            
            return Ok(city);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting City data");
            return StatusCode(500, "Error getting City data");
        }
    }
    
    [HttpPost("PostCity"), Authorize(Roles="Admin")]
    public async Task<ActionResult> PostCity(string name, double latitude, double longitude, string country, string? state)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(name);
            
            if (city != null)
            {
                return BadRequest("A city with that name already exists.");
            }
            
            _unitOfWork.CityRepository.Add(new City
            {
                Name = name, Latitude = latitude, Longitude = longitude, Country = country, State = state
            });
            
            _unitOfWork.Save();
            
            return Ok("Successfully added city.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error posting City data");
            return StatusCode(500, "Error posting City data");
        }
    }
    
    [HttpPatch("UpdateCity"), Authorize(Roles="Admin")]
    public async Task<ActionResult> UpdateCity(string name, double latitude, double longitude, string country, string? state)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(name);
            
            if (city == null)
            {
                return BadRequest("A city with that name doesn't exists.");
            }

            city.Name = name;
            city.Latitude = latitude;
            city.Longitude = longitude;
            city.Country = country;
            city.State = state;
            
            _unitOfWork.CityRepository.Update(city);
            _unitOfWork.Save();
            
            return Ok("Successfully updated city.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating City data");
            return StatusCode(500, "Error updating City data");
        }
    }
    
    [HttpDelete("DeleteCity"), Authorize(Roles="Admin")]
    public async Task<ActionResult> DeleteCity(string name)
    {
        try
        {
            City? city = _unitOfWork.CityRepository.GetByName(name);
            
            if (city == null)
            {
                return BadRequest("A city with that name doesn't exists.");
            }
            
            _unitOfWork.CityRepository.Delete(city);
            _unitOfWork.Save();
            
            return Ok("Successfully deleted city.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting City data");
            return StatusCode(500, "Error deleting City data");
        }
    }
}