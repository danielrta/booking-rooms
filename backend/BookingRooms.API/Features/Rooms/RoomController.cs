using BookingRooms.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Features.Rooms;

[Authorize]
[Route("api/rooms")]
public class RoomController(IRoomService roomService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRoomsAsync()
    {
        var result = await roomService.GetAllRoomsAsync();
        return FromResult(result);
    }

    [HttpGet("{id:int}", Name =  "GetRoomByIdAsync")]
    public async Task<IActionResult> GetRoomByIdAsync([FromRoute] int id)
    {
        var result = await roomService.GetRoomByIdAsync(id);
        return FromResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoomAsync(CreateRoomRequest request)
    {
        var result = await roomService.CreateRoomAsync(request);

        if (result.IsFailure)
        {
            return FromResult(result);
        }
        
        return CreatedAtRoute(
            routeName: "GetRoomByIdAsync",
            routeValues: new { id = result.Value.Id }, 
            value: result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoomAsync([FromRoute] int id, UpdateRoomRequest request)
    {
        var result = await roomService.UpdateRoomAsync(id, request);
        return FromResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoomAsync([FromRoute] int id)
    {
        var result = await roomService.DeleteRoomAsync(id);
        return FromResult(result);
    }
}