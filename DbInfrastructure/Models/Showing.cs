namespace DbInfrastructure.Models
{
    public class Showing
    {
        public int ShowingId { get; set; }
        public required int MovieId { get; set; }
        public required DateTime Date { get; set; }
        public required decimal Price { get; set; }

        public required Movie Movie { get; set; }
        public ICollection<ShowingSeat> ShowingSeats { get; set; } = new List<ShowingSeat>();
    }
}
