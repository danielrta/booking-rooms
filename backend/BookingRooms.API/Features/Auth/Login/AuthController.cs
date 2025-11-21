using BookingRooms.API.Common;
using BookingRooms.API.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Features.Auth;

[Route("api/auth")]
public class AuthController(IAuthService authService, IRegisterService registerService) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.Login(request);
        return FromResult(result);
    }
}