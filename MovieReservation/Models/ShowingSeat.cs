namespace MovieReservation.Models
{
    public class ShowingSeat
    {
        public int ShowingSeatId { get; set; }
        public required int ShowingId { get; set; }
        public required int SeatId { get; set; }

        public required Showing Showing { get; set; }
        public required Seat Seat { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
