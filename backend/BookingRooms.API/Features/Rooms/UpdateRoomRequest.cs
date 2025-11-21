namespace BookingRooms.API.Features.Rooms;

public record UpdateRoomRequest(string Name, int Capacity, string Location, List<int>? EquipmentIds);