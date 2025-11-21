using BookingRooms.API.Features.Rooms;

namespace BookingRooms.API.Features.Equipments;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public List<Room> Rooms { get; set; } = [];
}