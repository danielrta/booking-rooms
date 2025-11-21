using BookingRooms.API.Common.Results;

namespace BookingRooms.API.Features.Auth.Register;

public interface IRegisterService
{
    Task<Result> RegisterUserAsync(RegisterUserRequest request);
}