namespace MovieReservation.Models
{
    public class Showing
    {
        public int ShowingId { get; set; }
        public int MovieId { get; set; }
        public DateTime Date { get; set; }

        public Movie Movie { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; }
    }
}
