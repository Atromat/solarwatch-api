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

    private static DateTime GetDateTime(string dateAndTime)
    {
        var timeOnly = GetTimeOnlyFromSunsetSunriseString(dateAndTime);

        var dateOnlyString = string.Concat(dateAndTime.TakeWhile(c => c != 'T'));
        var splitDate = dateOnlyString.Split("-");
        var years = Int32.Parse(splitDate[0]);
        var months = Int32.Parse(splitDate[1]);
        var days = Int32.Parse(splitDate[2]);

        return new DateTime(years, months, days, timeOnly.Hour, timeOnly.Minute, timeOnly.Second);
    }
    
    public DateTime GetSunriseDateTime(string sunsetSunriseData)
    {
        JsonDocument json = JsonDocument.Parse(sunsetSunriseData);
        var sunrise = json.RootElement.GetProperty("results").GetProperty("sunrise").ToString();

        return GetDateTime(sunrise);
    }

    public DateTime GetSunsetDateTime(string sunsetSunriseData)
    {
        JsonDocument json = JsonDocument.Parse(sunsetSunriseData);
        var sunset = json.RootElement.GetProperty("results").GetProperty("sunset").ToString();

        return GetDateTime(sunset);
    }

    public int GetDayLength(string sunriseSunsetData)
    {
        JsonDocument json = JsonDocument.Parse(sunriseSunsetData);
        return json.RootElement.GetProperty("results").GetProperty("day_length").GetInt32();
    }
}