using BookingRooms.API.Features.Equipments;

namespace BookingRooms.API.Features.Rooms;

public record GetRoomResponse(int Id, string Name, int Capacity, string Location, List<EquipmentResponse> Equipments);