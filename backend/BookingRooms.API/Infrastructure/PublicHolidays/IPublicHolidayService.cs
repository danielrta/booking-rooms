namespace BookingRooms.API.Infrastructure;

public interface IPublicHolidayService
{
    
    Task<bool> IsPublicPublicHolidayAsync(
        DateOnly date,
        string countryCode,
        CancellationToken ct = default);
}