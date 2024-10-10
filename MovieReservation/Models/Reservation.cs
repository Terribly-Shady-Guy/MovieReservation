namespace MovieReservation.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public required int UserId { get; set; }
        public required DateTime DateReserved { get; set; }
        public required decimal Total { get; set; }
        public DateTime? DateCancelled { get; set; }

        public required AppUser User { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; } = new List<ShowingSeat>();
    }
}
