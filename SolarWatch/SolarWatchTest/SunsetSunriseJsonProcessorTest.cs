using SolarWatch.Services;

namespace SolarWatchTest;

public class SunsetSunriseJsonProcessorTest
{
    [Test]
    public void GetSunsetReturnProperTimeOnly()
    {
        var processor = new SunsetSunriseJsonProcessor();

        var timeOnly = processor.GetSunset(@$"{{
            ""results"": {{
                ""sunrise"": ""2024-01-22T04:53:43+00:00"",
                ""sunset"": ""2024-01-22T15:29:06+00:00"",
                ""solar_noon"": ""2024-01-22T10:11:24+00:00"",
                ""day_length"": 38123,
                ""civil_twilight_begin"": ""2024-01-22T04:29:31+00:00"",
                ""civil_twilight_end"": ""2024-01-22T15:53:17+00:00"",
                ""nautical_twilight_begin"": ""2024-01-22T04:00:26+00:00"",
                ""nautical_twilight_end"": ""2024-01-22T16:22:23+00:00"",
                ""astronomical_twilight_begin"": ""2024-01-22T03:31:48+00:00"",
                ""astronomical_twilight_end"": ""2024-01-22T16:51:00+00:00""
            }},
            ""status"": ""OK"",
            ""tzid"": ""UTC""
        }}");
        
        Console.WriteLine($"timeonly: {timeOnly.Hour}:{timeOnly.Minute}:{timeOnly.Second}");
        
        Assert.That(timeOnly.Hour == 15 && timeOnly.Minute == 29 && timeOnly.Second == 6);
    }
    
    [Test]
    public void GetSunriseReturnProperTimeOnly()
    {
        var processor = new SunsetSunriseJsonProcessor();

        var timeOnly = processor.GetSunrise(@$"{{
            ""results"": {{
                ""sunrise"": ""2024-01-22T04:53:43+00:00"",
                ""sunset"": ""2024-01-22T15:29:06+00:00"",
                ""solar_noon"": ""2024-01-22T10:11:24+00:00"",
                ""day_length"": 38123,
                ""civil_twilight_begin"": ""2024-01-22T04:29:31+00:00"",
                ""civil_twilight_end"": ""2024-01-22T15:53:17+00:00"",
                ""nautical_twilight_begin"": ""2024-01-22T04:00:26+00:00"",
                ""nautical_twilight_end"": ""2024-01-22T16:22:23+00:00"",
                ""astronomical_twilight_begin"": ""2024-01-22T03:31:48+00:00"",
                ""astronomical_twilight_end"": ""2024-01-22T16:51:00+00:00""
            }},
            ""status"": ""OK"",
            ""tzid"": ""UTC""
        }}");
        
        Console.WriteLine($"timeonly: {timeOnly.Hour}:{timeOnly.Minute}:{timeOnly.Second}");
        
        Assert.That(timeOnly.Hour == 4 && timeOnly.Minute == 53 && timeOnly.Second == 43);
    }
    
    [Test]
    public void GetSunriseReturnProperDateTime()
    {
        var processor = new SunsetSunriseJsonProcessor();

        var dateTime = processor.GetSunriseDateTime(@$"{{
            ""results"": {{
                ""sunrise"": ""2024-01-22T04:53:43+00:00"",
                ""sunset"": ""2024-01-22T15:29:06+00:00"",
                ""solar_noon"": ""2024-01-22T10:11:24+00:00"",
                ""day_length"": 38123,
                ""civil_twilight_begin"": ""2024-01-22T04:29:31+00:00"",
                ""civil_twilight_end"": ""2024-01-22T15:53:17+00:00"",
                ""nautical_twilight_begin"": ""2024-01-22T04:00:26+00:00"",
                ""nautical_twilight_end"": ""2024-01-22T16:22:23+00:00"",
                ""astronomical_twilight_begin"": ""2024-01-22T03:31:48+00:00"",
                ""astronomical_twilight_end"": ""2024-01-22T16:51:00+00:00""
            }},
            ""status"": ""OK"",
            ""tzid"": ""UTC""
        }}");
        
        Console.WriteLine($"dateTime: {dateTime.Year}-{dateTime.Month}-{dateTime.Day}T{dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}");
        
        Assert.That(dateTime.Year == 2024 && dateTime.Month == 01 && dateTime.Day == 22 && dateTime.Hour == 4 && dateTime.Minute == 53 && dateTime.Second == 43);
    }
}