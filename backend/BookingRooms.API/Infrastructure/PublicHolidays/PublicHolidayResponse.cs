namespace BookingRooms.API.Infrastructure;

public class PublicHolidayResponse
{
    public DateOnly Date { get; init; }
    public string LocalName { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string CountryCode { get; init; } = null!;
    public bool Fixed { get; init; }
    public bool Global { get; init; }
    public string[] Types { get; init; } = [];
}