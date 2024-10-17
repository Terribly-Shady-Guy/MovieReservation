namespace MovieReservation.ViewModels
{
    public class MovieUploadVM
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required IFormFile PosterImage { get; set; }
        public required string Genre { get; set; }
    }
}
