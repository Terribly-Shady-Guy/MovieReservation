namespace DbInfrastructure.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string PosterImageName { get; set; }
        public required string Genre { get; set; }

        public ICollection<Showing> Showings { get; set; } = new List<Showing>();
    }
}
