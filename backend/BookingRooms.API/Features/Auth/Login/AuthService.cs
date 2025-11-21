using BookingRooms.API.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace BookingRooms.API.Features.Auth;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IAuthService
{
    public async Task<Result<LoginResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.UserName);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<LoginResponse>.Failure(Error.Unauthorized("Invalid username or password."));
        }
        
        var roles = await userManager.GetRolesAsync(user);
        var token = JwtTokenGenerator.GenerateToken(user, roles, configuration);

        var response = new LoginResponse(user.UserName, token.Item2, token.Item1, roles.ToList());
        
        return Result<LoginResponse>.Success(response);
    }
}