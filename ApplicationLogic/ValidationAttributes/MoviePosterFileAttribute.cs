using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed partial class MoviePosterFileAttribute : ValidationAttribute
    {
        private static readonly Dictionary<string, byte[]> _validSignatures = new()
        {
            [".jpg"] = [0xFF, 0xD8],
            [".jpeg"] = [0xFF, 0xD8],
            [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        };

        const int FileSizeLimitInBytes = 10 * 1024 * 1024;
        private static readonly string _invalidFileTypeErrorMessage = $"This is not a valid file type. File type must be one of the following: {string.Join(", ", _validSignatures.Keys)}.";

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file)
            {
                return ValidationResult.Success;
            }

            if (file.Length > FileSizeLimitInBytes)
            {
                return new ValidationResult($"The uploaded file must be {FileSizeLimitInBytes / (1024 * 1024)}mb or smaller.");
            }

            if (!CreateFileNameValidationRegex().IsMatch(file.FileName))
            {
                return new ValidationResult("The file name is too long or contains illegal characters. The filename must be between 5 and 100 characters and only contain alphanumeric and the following special characters: -, _, ., and whitespace.");
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            if (!_validSignatures.TryGetValue(extension, out byte[]? validSignature))
            {
                return new ValidationResult(_invalidFileTypeErrorMessage);
            }

            using Stream fileContent = file.OpenReadStream();

            byte[] fileHeader = new byte[validSignature.Length];
            fileContent.ReadAtLeast(fileHeader, fileHeader.Length, false);

            if (!fileHeader.SequenceEqual(validSignature))
            {
                return new ValidationResult(_invalidFileTypeErrorMessage);
            }

            return ValidationResult.Success;
        }

        [GeneratedRegex("^[a-zA-Z0-9-_. ]{5,100}$")]
        private static partial Regex CreateFileNameValidationRegex();
    }
}
