namespace DbInfrastructure.Models
{
    public enum ReservationStatus { Pending, Confirmed, CheckedIn, Cancelled }
    public class Reservation
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required DateTime DateReserved { get; set; }
        public required ReservationStatus Status { get; set; }
        public DateTime? DateCancelled { get; set; }

        public required AppUser User { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; } = new List<ShowingSeat>();
        public required ReservationStatusLookup StatusLookup { get; set; }
    }
}
