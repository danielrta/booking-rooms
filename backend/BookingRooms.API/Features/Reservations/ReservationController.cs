using BookingRooms.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Features.Reservations;

[Authorize]
[Route("api/reservations")]
public class ReservationController(IReservationService reservationService) : ApiControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetAllReservationsAsync()
    {
        if (IsAdmin)
        {
            var adminResult = await reservationService.GetAllReservationsAsync();
            return FromResult(adminResult);
        }
        
        var userResult = await reservationService.GetReservationsByUserAsync(UserId);
        return FromResult(userResult);
    }
    
    [HttpGet("{id:int}", Name = "GetReservationByIdAsync")]
    public async Task<IActionResult> GetReservationByIdAsync(int id)
    {
        var result = await reservationService.GetReservationAsync(id);
        return FromResult(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateReservationAsync(CreateReservationRequest request)
    {
        var result = await reservationService.CreateReservationAsync(request, UserId);

        if (result.IsFailure)
        {
            return FromResult(result);
        }
        
        return CreatedAtRoute(
            routeName: "GetReservationByIdAsync",
            routeValues: new { id = result.Value.Id }, 
            value: result.Value);
    }
}