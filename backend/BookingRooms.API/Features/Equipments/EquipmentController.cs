using BookingRooms.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingRooms.API.Features.Equipments;

[Authorize(Roles = "Admin")]
[Route("api/equipments")]
public class EquipmentController(IEquipmentService equipmentService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await equipmentService.GetAllAsync();
        
        return FromResult(result);
    }
}