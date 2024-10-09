namespace MovieReservation.Models
{
    public class ShowingSeat
    {
        public int ShowingSeatId { get; set; }
        public int ShowingId { get; set; }
        public int SeatId { get; set; }

        public Showing Showing { get; set; }
        public Seat Seat { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
