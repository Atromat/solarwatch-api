using Microsoft.AspNetCore.Mvc;
using SolarWatch.Services;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController : ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IJsonProcessor _jsonProcessor;
    private readonly ICoordDataProvider _coordDataProvider;
    private readonly ISunsetSunriseDataProvider _sunsetSunriseDataProvider;
    private readonly ISunsetSunriseJsonProcessor _sunsetSunriseJsonProcessor;

    public SolarWatchController(ILogger<SolarWatchController> logger, IJsonProcessor jsonProcessor, 
        ICoordDataProvider coordDataProvider, ISunsetSunriseDataProvider sunsetSunriseDataProvider,
        ISunsetSunriseJsonProcessor sunsetSunriseJsonProcessor)
    {
        _logger = logger;
        _jsonProcessor = jsonProcessor;
        _coordDataProvider = coordDataProvider;
        _sunsetSunriseDataProvider = sunsetSunriseDataProvider;
        _sunsetSunriseJsonProcessor = sunsetSunriseJsonProcessor;
    }

    [HttpGet("GetSunriseTime")]
    public async Task<ActionResult<TimeOnly>> GetSunriseTime(string cityName)
    {
        try
        {
            var cityData = await _coordDataProvider.GetDataByCity(cityName);
            var cityLatLon = _jsonProcessor.GetLongitudeLatitude(cityData);
            var sunriseData = _sunsetSunriseDataProvider.GetDataByLongitudeLatitude(cityLatLon.Item1, cityLatLon.Item2);
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
            var cityData = await _coordDataProvider.GetDataByCity(cityName);
            var cityLatLon = _jsonProcessor.GetLongitudeLatitude(cityData);
            var sunsetData = _sunsetSunriseDataProvider.GetDataByLongitudeLatitude(cityLatLon.Item1, cityLatLon.Item2);
            var sunsetTime = _sunsetSunriseJsonProcessor.GetSunset(sunsetData);
            return Ok(sunsetTime);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sun rise data");
            return NotFound("Error getting sun rise data");
        }
    }
}