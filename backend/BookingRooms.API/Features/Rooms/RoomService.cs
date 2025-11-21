using BookingRooms.API.Common.Results;
using BookingRooms.API.Data;
using BookingRooms.API.Features.Equipments;
using BookingRooms.API.Features.Reservations;
using Microsoft.EntityFrameworkCore;

namespace BookingRooms.API.Features.Rooms;

public class RoomService(BookingContext context) : IRoomService
{
    public async Task<Result<List<GetRoomResponse>>> GetAllRoomsAsync()
    {
        var rooms = await context.Rooms
            .AsNoTracking()
            .Include(r=> r.Equipments)
            .ToListAsync();

        var response = rooms
            .Select(r => new GetRoomResponse(r.Id, r.Name, r.Capacity, r.Location,
                r.Equipments.Select(e => new EquipmentResponse(e.Id, e.Name))
                    .ToList()))
            .ToList();

        return Result<List<GetRoomResponse>>.Success(response);
    }

    public async Task<Result<GetRoomResponse>> GetRoomByIdAsync(int id)
    {
        var room = await context.Rooms
            .AsNoTracking()
            .Include(r => r.Equipments)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room is null)
        {
            return Result<GetRoomResponse>.Failure(Error.NotFound("Room", id));
        }

        var response = new GetRoomResponse(
            room.Id,
            room.Name,
            room.Capacity,
            room.Location,
            room.Equipments.Select(e => new EquipmentResponse(e.Id, e.Name)).ToList()
        );

        return Result<GetRoomResponse>.Success(response);
    }

    public async Task<Result<GetRoomResponse>> CreateRoomAsync(CreateRoomRequest request)
    {
        var newRoom = request.ToRoom();
        
        if(request.EquipmentIds is not null && request.EquipmentIds.Count != 0)
        {
            newRoom.Equipments = [];
            
            var equipments = await context.Equipments
                .Where(e => request.EquipmentIds.Contains(e.Id))
                .ToListAsync();
            newRoom.Equipments.AddRange(equipments);
        }
        
        context.Rooms.Add(newRoom);
        await context.SaveChangesAsync();
        
        await context.Entry(newRoom).Collection(r=> r.Equipments).LoadAsync();

        return Result<GetRoomResponse>.Success(newRoom.ToGetRoomResponse());
    }

    public async Task<Result> UpdateRoomAsync(int id, UpdateRoomRequest request)
    {
        var existingRoom = await context.Rooms
            .Include(r=> r.Equipments)
            .FirstOrDefaultAsync(r=> r.Id == id);

        if (existingRoom is null)
        {
            return Result.Failure(Error.NotFound("Room", id));
        }

        var updatedRoom = request.ToRoom(existingRoom);
        
        var equipments = await context.Equipments
            .Where(e => request.EquipmentIds.Contains(e.Id))
            .ToListAsync();
        
        updatedRoom.Equipments = equipments;
        context.Rooms.Update(updatedRoom);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> DeleteRoomAsync(int id)
    {
        var existingRoom = await context.Rooms.FindAsync(id);

        if (existingRoom is null)
        {
            return Result.Failure(Error.NotFound("Room", id));
        }

        var hasActiveReservations = await context.Reservations
            .AnyAsync(r => r.RoomId == id && r.Status != ReservationStatus.Cancelled);
        
        if (hasActiveReservations)
        {
            return Result.Failure(Error.Validation("Cannot delete room with active reservations."));
        }

        context.Rooms.Remove(existingRoom);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }
}