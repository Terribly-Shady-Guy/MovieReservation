using System.ComponentModel.DataAnnotations;

namespace MovieReservation.ViewModels
{
    public class MovieFormDataBody
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        [ImageFileValidation]
        public required IFormFile PosterImage { get; set; }
        public required string Genre { get; set; }
    }

    public sealed class ImageFileValidationAttribute : ValidationAttribute
    {
        private static readonly string[] _validExtensions = [".jpg", ".png", ".jpeg"];
        private const int _fileSizeLimitInMB = 10;

        public override bool IsValid(object? value)
        {
            var file = (IFormFile?)value;
            if (file == null) return false;

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_validExtensions.Any(ext => ext == extension))
            {
                ErrorMessage = $"This is not a valid file type. File type must be one of the following: {string.Join(", ", _validExtensions)}.";
                return false;
            }

            long imageSizeInMB = file.Length / (1024 * 1024);

            if (imageSizeInMB > _fileSizeLimitInMB)
            {
                ErrorMessage = $"The uploaded file must be {_fileSizeLimitInMB}mb or smaller.";
                return false;
            }
            
            return true;
        }
    }
}
