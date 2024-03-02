using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SolarWatch.Contracts;

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
        var request = new AuthRequest("sim@gmail.com", "simsim");
        
        //Act
        await _client.PostAsJsonAsync("Auth/Login", request);
        var response = await _client.GetAsync("SolarWatch/GetSunriseSunset?cityName=Delhi&year=2021&month=3&day=3");
        await _client.PostAsJsonAsync("Auth/Logout", request);
        
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
        var request = new AuthRequest("sim@gmail.com", "simsim");
        
        //Act
        await _client.PostAsJsonAsync("Auth/Login", request);
        var response = await _client.GetAsync("SolarWatch/GetSunriseSunset?cityName=Delhi&year=2019&month=9&day=9");
        await _client.PostAsJsonAsync("Auth/Logout", request);
        
        //Assert
        JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var sunsetId = json.RootElement.GetProperty("id").ToString();
        var sunrise = json.RootElement.GetProperty("sunrise").ToString();
        var sunset = json.RootElement.GetProperty("sunset").ToString();
        var dayLength = json.RootElement.GetProperty("dayLength").ToString();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(22.ToString(), sunsetId);
        Assert.Equal("2019-09-09T00:32:58", sunrise);
        Assert.Equal("2019-09-09T13:06:00", sunset);
        Assert.Equal(45182.ToString(), dayLength);
    }
}