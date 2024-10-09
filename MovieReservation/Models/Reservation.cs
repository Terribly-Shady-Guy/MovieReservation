namespace MovieReservation.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public DateTime DateReserved { get; set; }
        public decimal Total { get; set; }
        public DateTime? DateCancelled { get; set; }

        public AppUser User { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; }
    }
}
