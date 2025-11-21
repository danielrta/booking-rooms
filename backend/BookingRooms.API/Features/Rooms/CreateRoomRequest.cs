namespace BookingRooms.API.Features.Rooms;

public record CreateRoomRequest(string Name, int Capacity, string Location, List<int>? EquipmentIds);