using BookingRooms.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Features.Auth.Register;

[Authorize(Roles = "Admin")]
[Route("api/auth/register")]
public class RegisterController(IRegisterService registerService) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        var result = await registerService.RegisterUserAsync(request);
        return FromResult(result);
    }
}