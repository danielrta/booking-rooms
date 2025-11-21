using BookingRooms.API.Features.Auth;
using BookingRooms.API.Features.Rooms;

namespace BookingRooms.API.Features.Reservations;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; }

    public string UserId { get; set; } = null!;
    
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; }
}