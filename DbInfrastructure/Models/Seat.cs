namespace DbInfrastructure.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public required string AuditoriumNumber { get; set; }

        public required Auditorium Auditorium { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; } = new List<ShowingSeat>();
    }
}
