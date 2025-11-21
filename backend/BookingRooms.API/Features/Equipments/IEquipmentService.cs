using BookingRooms.API.Common.Results;

namespace BookingRooms.API.Features.Equipments;

public interface IEquipmentService
{
    Task<Result<List<EquipmentResponse>>> GetAllAsync();
    Task<Result<EquipmentResponse>> GetByIdAsync(int id);
}