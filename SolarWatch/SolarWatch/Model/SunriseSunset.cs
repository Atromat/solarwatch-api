namespace SolarWatch.Model;

public class SunriseSunset
{
    public City City { get; init; }
    public DateTime Sunrise { get; init; }
    public DateTime Sunset { get; init; }
    public int DayLength { get; init; }
}