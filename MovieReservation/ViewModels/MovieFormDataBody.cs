using DbInfrastructure.ValidationAttributes;

namespace DbInfrastructure.ViewModels
{
    public class MovieFormDataBody
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        [MoviePosterFile]
        public required IFormFile PosterImage { get; set; }
        public required string Genre { get; set; }
    }
}
