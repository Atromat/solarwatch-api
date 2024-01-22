using System.Text.Json;

namespace SolarWatch.Services;

public class SunsetSunriseJsonProcessor : ISunsetSunriseJsonProcessor
{
    public TimeOnly GetSunrise(string sunsetSunriseData)
    {
        JsonDocument json = JsonDocument.Parse(sunsetSunriseData);
        var sunrise = json.RootElement.GetProperty("results").GetProperty("sunrise").ToString();

        return GetTimeOnlyFromSunsetSunriseString(sunrise);
    }

    public TimeOnly GetSunset(string sunsetSunriseData)
    {
        JsonDocument json = JsonDocument.Parse(sunsetSunriseData);
        var sunset = json.RootElement.GetProperty("results").GetProperty("sunset").ToString();

        return GetTimeOnlyFromSunsetSunriseString(sunset);
    }
    
    private static TimeOnly GetTimeOnlyFromSunsetSunriseString(string dateAndTime)
    {
        var timeOnlyString = string.Concat(dateAndTime.SkipWhile(c => c != 'T').Skip(1).TakeWhile(c => c != '+'));
        var splitTime = timeOnlyString.Split(":");
        var hours = Int32.Parse(splitTime[0]);
        var minutes = Int32.Parse(splitTime[1]);
        var seconds = Int32.Parse(splitTime[2]);

        return new TimeOnly(hours,minutes,seconds);
    }
}