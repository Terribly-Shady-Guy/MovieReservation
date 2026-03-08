using System.ComponentModel.DataAnnotations;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoviePosterFileAttribute : ValidationAttribute
    {
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

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                if (_validSignatures.TryGetValue(extension, out byte[]? validSignature) && !validSignature.SequenceEqual(reader.ReadBytes(validSignature.Length)))
                {
                    return new ValidationResult("This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.");
                }
                else if (validSignature is null)
                {
                    return new ValidationResult("This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.");
                }
            }

            if (file.Length > _FileSizeLimitInBytes)
            {
                return new ValidationResult($"The uploaded file must be {_FileSizeLimitInBytes / (1024 * 1024)}mb or smaller.");
            }

            return ValidationResult.Success;
        }
    }
}
