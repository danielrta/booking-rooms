using BookingRooms.API.Features.Equipments;

namespace BookingRooms.API.Features.Rooms;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
    
    public List<Equipment> Equipments { get; set; } = [];
}