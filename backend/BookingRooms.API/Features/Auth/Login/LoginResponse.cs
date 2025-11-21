namespace BookingRooms.API.Features.Auth;

public sealed record LoginResponse
(
    string UserName,
    string Token,
    DateTime ExpiresAt,
    List<string> Roles
);