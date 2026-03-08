using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoviePosterFileAttribute : ValidationAttribute
    {
        private static readonly Dictionary<string, byte[]> _validSignatures = new()
        {
            [".jpg"] = [0xFF, 0xD8],
            [".jpeg"] = [0xFF, 0xD8],
            [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file)
            {
                return ValidationResult.Success;
            }

            if (!Regex.IsMatch(file.FileName, "^[a-zA-Z0-9-_.]+$"))
            {
                return new ValidationResult("The file name contains illegal characters.");
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                string invalidFileTypeErrorMessage = $"This is not a valid file type. File type must be one of the following: {string.Join(", ", _validSignatures.Keys)}.";
                if (!_validSignatures.TryGetValue(extension, out byte[]? validSignature))
                {
                    return new ValidationResult(invalidFileTypeErrorMessage);
                }

                byte[] fileSignature = reader.ReadBytes(validSignature.Length);
                if (!fileSignature.SequenceEqual(validSignature))
                {
                    return new ValidationResult(invalidFileTypeErrorMessage);
                }
            }

            const int FileSizeLimitInBytes = 10 * 1024 * 1024;
            if (file.Length > FileSizeLimitInBytes)
            {
                return new ValidationResult($"The uploaded file must be {FileSizeLimitInBytes / (1024 * 1024)}mb or smaller.");
            }

            return ValidationResult.Success;
        }
    }
}
