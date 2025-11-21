namespace BookingRooms.API.Features.Reservations;

public static class ReservationMappings
{
    public static GetReservationResponse ToDto(this Reservation reservation)
    {
        return new GetReservationResponse(
            reservation.Id,
            reservation.RoomId,
            reservation.Room.Name,
            reservation.UserId,
            reservation.User.Email,
            DateTime.SpecifyKind(reservation.StartTimeUtc, DateTimeKind.Utc),
            DateTime.SpecifyKind(reservation.EndTimeUtc,   DateTimeKind.Utc),
            reservation.Status.ToString());
    }

    public static Reservation ToReservation(this CreateReservationRequest request, string userId, DateTime startUtc,
        DateTime endUtc)
    {
        return new Reservation
        {
            RoomId = request.RoomId,
            StartTimeUtc = startUtc,
            EndTimeUtc = endUtc,
            UserId = userId,
        };
    }
}