namespace MovieReservation.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public string AuditoriumNumber { get; set; }
        public decimal Price { get; set; }

        public Auditorium Auditorium { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; }
    }
}
