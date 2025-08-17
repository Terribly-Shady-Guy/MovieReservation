using DbInfrastructure.Models;

namespace ApplicationLogic.ViewModels
{
    public class MovieDto
    {
        public required int MovieId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ImageFileName { get; set; }
        public required List<string> Genres { get; set; }
        public string ImageLink { get; set; } = string.Empty;
    }
}
