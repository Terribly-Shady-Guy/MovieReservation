namespace MovieReservation.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PosterImageName { get; set; }
        public string Genre { get; set; }

        public ICollection<Showing> Showings { get; set; }
    }
}
