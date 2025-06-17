using ApplicationLogic.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace ApplicationLogic.ViewModels
{
    public class MovieFormDataBody
    {
        [MinLength(1)]
        public required string Title { get; set; }
        [MinLength(1)]
        public required string Description { get; set; }
        [MoviePosterFile]
        public required IFormFile PosterImage { get; set; }
        [MinLength(1)]
        public required string Genre { get; set; }
    }
}
