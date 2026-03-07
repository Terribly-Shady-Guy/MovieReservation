using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoviePosterFileAttribute : ValidationAttribute
    {
        private readonly Dictionary<string, string[]> _validTypes = new()
        {
            [MediaTypeNames.Image.Jpeg] = [".jpg", ".jpeg"],
            [MediaTypeNames.Image.Png] = [".png"],
        };

        private readonly Dictionary<string, byte[]> _validSignatures = new()
        {
            [".jpg"] = [0xFF, 0xD8],
            [".jpeg"] = [0xFF, 0xD8],
            [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        };

        private const int _FileSizeLimitInBytes = 10 * 1024 * 1024;

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file)
            {
                return ValidationResult.Success;
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();
            
            if (!_validTypes.TryGetValue(file.ContentType, out string[]? fileTypes) || !fileTypes.Contains(extension))
            {
                return new ValidationResult("This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.");
            }

            var reader = new BinaryReader(file.OpenReadStream());
            byte[] signature = _validSignatures[extension];

            if (!signature.SequenceEqual(reader.ReadBytes(signature.Length)))
            {
                return new ValidationResult("This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.");
            }

            if (file.Length > _FileSizeLimitInBytes)
            {
                return new ValidationResult($"The uploaded file must be {_FileSizeLimitInBytes / (1024 * 1024)}mb or smaller.");
            }

            return ValidationResult.Success;
        }
    }
}
