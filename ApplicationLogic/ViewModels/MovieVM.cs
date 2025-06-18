using DbInfrastructure.Models;

namespace ApplicationLogic.ViewModels
{
    public class MovieVM
    {
        public required int MovieId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string PosterImageName { get; set; }
        public required List<string> Genres { get; set; }
    }
}
