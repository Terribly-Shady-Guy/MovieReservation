namespace MovieReservation.Models
{
    public class Auditorium
    {
        public string AuditoriumNumber { get; set; }
        public int MaxCapacity { get; set; }
        public int LocationId { get; set; }

        public Location Location { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
