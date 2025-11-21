using BookingRooms.API.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace BookingRooms.API.Features.Auth.Register;

public class RegisterService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    : IRegisterService
{
    public async Task<Result> RegisterUserAsync(RegisterUserRequest request)
    {
        var existingUser = await userManager.FindByNameAsync(request.UserName);

        if (existingUser is not null)
        {
            return Result.Failure(Error.Conflict("User already exists."));
        }

        var newUser = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.UserName
        };

        var createUserResult = await userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {
            var errors = string.Join("; ", createUserResult.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation(errors));
        }

        if (!await roleManager.RoleExistsAsync(request.Role))
        {
            return Result.Failure(Error.Conflict("Invalid role: " + request.Role));
        }

        var addToRoleResult = await userManager.AddToRoleAsync(newUser, request.Role);
        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join("; ", addToRoleResult.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation(errors));
        }


        return Result.Success();
    }
}