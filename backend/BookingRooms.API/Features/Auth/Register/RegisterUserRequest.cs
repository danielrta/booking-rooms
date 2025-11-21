namespace BookingRooms.API.Features.Auth.Register;

public record RegisterUserRequest(string UserName, string Password, string Role);