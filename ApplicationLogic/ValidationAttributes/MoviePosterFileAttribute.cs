using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoviePosterFileAttribute : ValidationAttribute
    {
        private readonly Dictionary<string, string> _validTypes = new()
        {
            [".jpg"] = MediaTypeNames.Image.Jpeg,
            [".jpeg"] = MediaTypeNames.Image.Jpeg,
            [".png"] = MediaTypeNames.Image.Png
        };

        private const int _fileSizeLimitInMB = 10;

        public override bool IsValid(object? value)
        {
            if (value is null || value is not IFormFile file)
            {
                return false;
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();
            
            if (!_validTypes.TryGetValue(extension, out string? mimeType) || mimeType != file.ContentType)
            {
                ErrorMessage = $"This is not a valid file type. File type must be one of the following: {string.Join(", ", _validTypes.Keys)}.";
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
