using BookingRooms.API.Common.Results;

namespace BookingRooms.API.Features.Rooms;

public interface IRoomService
{
    Task<Result<List<GetRoomResponse>>> GetAllRoomsAsync();
    Task<Result<GetRoomResponse>> GetRoomByIdAsync(int id);
    Task<Result<GetRoomResponse>> CreateRoomAsync(CreateRoomRequest request);
    Task<Result> UpdateRoomAsync(int id, UpdateRoomRequest request);
    Task<Result> DeleteRoomAsync(int id);
}