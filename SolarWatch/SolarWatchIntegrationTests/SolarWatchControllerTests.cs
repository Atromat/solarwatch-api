using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SolarWatch.Contracts;
using SolarWatch.Model;

namespace SolarWatchIntegrationTests;

public class SolarWatchControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SolarWatchControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetSunriseSunsetReturnsOk()
    {
        //Arrange
        var registerRequest = new RegistrationRequest("sim@gmail.com", "sim", "simsim");
        await _client.PostAsJsonAsync("Auth/Register", registerRequest);
        var loginRequest = new AuthRequest("sim@gmail.com", "simsim");
        await _client.PostAsJsonAsync("Auth/Login", loginRequest);
        
        //Act
        var response = await _client.GetAsync("SolarWatch/GetSunriseSunset?cityName=Delhi&year=2021&month=3&day=3");
        await _client.PostAsJsonAsync("Auth/Logout", loginRequest);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsUnauthorized()
    {
        //Arrange
        
        //Act
        var response = await _client.GetAsync("SolarWatch/GetSunriseSunset?cityName=Delhi&year=2021&month=3&day=3");
        
        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsProperValuesForInput()
    {
        //Arrange
        var registerRequest = new RegistrationRequest("sim@gmail.com", "sim", "simsim");
        await _client.PostAsJsonAsync("Auth/Register", registerRequest);
        var loginRequest = new AuthRequest("sim@gmail.com", "simsim");
        await _client.PostAsJsonAsync("Auth/Login", loginRequest);
        
        //Act
        var response = await _client.GetAsync("SolarWatch/GetSunriseSunset?cityName=Delhi&year=2019&month=9&day=9");
        await _client.PostAsJsonAsync("Auth/Logout", loginRequest);
        
        //Assert
        var sunriseSunset = await response.Content.ReadFromJsonAsync<SunriseSunset>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new DateTime(2019, 09, 09, 0, 32, 58), sunriseSunset.Sunrise);
        Assert.Equal(new DateTime(2019, 09, 09, 13, 6, 0), sunriseSunset.Sunset);
        Assert.Equal(45182, sunriseSunset.DayLength);
    }
}