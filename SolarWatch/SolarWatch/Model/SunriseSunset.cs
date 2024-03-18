namespace SolarWatch.Model;

public class SunriseSunset
{
    public int Id { get; init; }
    public City City { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public int DayLength { get; set; }
}