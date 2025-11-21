using BookingRooms.API.Infrastructure;

namespace BookingRooms.Specs.Integration.Reservations;

public class FakePublicHolidayService : IPublicHolidayService
{
    public HashSet<DateOnly> PublicHolidays { get; } = [];
    public Task<bool> IsPublicPublicHolidayAsync(DateOnly date, string countryCode, CancellationToken ct = default)
    {
        var list = PublicHolidays
            .Where(d => d.Year == date.Year)
            .Select(d => new PublicHolidayResponse()
            {
                Date = d,
                LocalName = "Fake Holiday",
                Name = "Fake Holiday",
                CountryCode = countryCode,
                Fixed = false,
                Global = true,
                Types = ["Public"]
            })
            .ToList();
        
        return Task.FromResult(list.Any(d => d.Date == date));
    }
}