using Microsoft.Extensions.Caching.Memory;

namespace BookingRooms.API.Infrastructure;

public sealed class PublicHolidayService(HttpClient httpClient, IMemoryCache cache) : IPublicHolidayService
{
    
    public async Task<bool> IsPublicPublicHolidayAsync(
        DateOnly date,
        string countryCode,
        CancellationToken ct = default)
    {
        var holidays = await GetPublicHolidaysAsync(date.Year, countryCode, ct);

        return holidays.Any(h =>
            h.Date == date &&
            h.Types.Any(t => string.Equals(t, "Public", StringComparison.OrdinalIgnoreCase)));
    }
    
    private async Task<IReadOnlyList<PublicHolidayResponse>> GetPublicHolidaysAsync(
        int year,
        string countryCode,
        CancellationToken ct = default)
    {
        var cacheKey = $"PublicHolidays:{countryCode}:{year}";

        if (cache.TryGetValue(cacheKey, out IReadOnlyList<PublicHolidayResponse>? cached)
            && cached is not null)
        {
            return cached;
        }

        var url = $"/api/v3/PublicHolidays/{year}/{countryCode}";

        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var holidays = await response.Content.ReadFromJsonAsync<List<PublicHolidayResponse>>(cancellationToken: ct)
                       ?? [];
        
        cache.Set(cacheKey, holidays, TimeSpan.FromHours(12));

        return holidays;
    }
}