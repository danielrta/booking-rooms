namespace BookingRooms.API.Features.Auth;

public sealed record LoginRequest
(
    string UserName,
    string Password
);