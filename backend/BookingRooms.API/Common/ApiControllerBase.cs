using System.Security.Claims;
using BookingRooms.API.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Common;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    protected bool IsAdmin => User.IsInRole("Admin");
    protected IActionResult FromResult(Result result)
    {
        return result.IsSuccess ? NoContent() :
            MapErrorToActionResult(result.Error);
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return MapErrorToActionResult(result.Error);
    }

    protected IActionResult FromResult<T>(
        Result<T> result,
        Func<T?, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value);
        }

        return MapErrorToActionResult(result.Error);
    }

    private ObjectResult MapErrorToActionResult(Error error)
    {
        return error.Code switch
        {
            "ValidationError" => BadRequest(error.Description),
            "NotFound"        => NotFound(error.Description),
            "Conflict"        => Conflict(error.Description),
            "Unauthorized"    => StatusCode(StatusCodes.Status401Unauthorized, error.Description),
            _                 => StatusCode(StatusCodes.Status500InternalServerError, error.Description)
        };
    }
}