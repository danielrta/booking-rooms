using BookingRooms.API.Features.Equipments;

namespace BookingRooms.API.Features.Rooms;

public static class RoomMappings
{
    public static GetRoomResponse ToGetRoomResponse(this Room room)
    {
        return new GetRoomResponse(
            room.Id,
            room.Name,
            room.Capacity,
            room.Location,
            room.Equipments.Select(e => new EquipmentResponse(e.Id, e.Name)).ToList()
        );
    }

    public static Room ToRoom(this CreateRoomRequest request)
    {
        return new Room
        {
            Name = request.Name,
            Capacity = request.Capacity,
            Location = request.Location
        };
    }
    
    public static Room ToRoom(this UpdateRoomRequest request, Room room)
    {
        room.Name = request.Name;
        room.Capacity = request.Capacity;
        room.Location = request.Location;
        return room;
    }
}