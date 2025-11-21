using BookingRooms.API.Common.Results;

namespace BookingRooms.API.Features.Auth;

public interface IAuthService
{
    Task<Result<LoginResponse>> Login(LoginRequest request);
}