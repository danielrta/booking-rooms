using BookingRooms.API.Common.Results;
using BookingRooms.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingRooms.API.Features.Equipments;

public sealed class EquipmentService(BookingContext context) : IEquipmentService
{
    public async Task<Result<List<EquipmentResponse>>> GetAllAsync()
    {
        var equipments = await context.Equipments
            .AsNoTracking()
            .ToListAsync();
        
        var response = equipments
            .Select(e => new EquipmentResponse(e.Id, e.Name))
            .ToList();
        
        return Result<List<EquipmentResponse>>.Success(response);
    }

    public async Task<Result<EquipmentResponse>> GetByIdAsync(int id)
    {
        var equipment = await context.Equipments
            .Where(e => e.Id == id)
            .Select(e => new EquipmentResponse(e.Id, e.Name))
            .FirstOrDefaultAsync()!;

        return equipment == null
            ? Result<EquipmentResponse>.Failure(Error.NotFound("Equipment", id))
            : Result<EquipmentResponse>.Success(equipment);
    }
}