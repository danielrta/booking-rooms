namespace BookingRooms.API.Features.Reservations;

public record GetReservationResponse(
    int Id,
    int RoomId,
    string RoomName,
    string UserId,
    string UserName,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    string Status
);