namespace MovieReservation.Infrastructure.Models
{
    public class Auditorium
    {
        public required string AuditoriumNumber { get; set; }
        public required int MaxCapacity { get; set; }
        public required int LocationId { get; set; }

        public required Location Location { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
