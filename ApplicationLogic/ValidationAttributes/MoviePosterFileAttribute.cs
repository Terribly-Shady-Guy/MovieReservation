﻿using System.ComponentModel.DataAnnotations;

namespace ApplicationLogic.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoviePosterFileAttribute : ValidationAttribute
    {
        private static readonly string[] _validExtensions = [".jpg", ".png", ".jpeg"];
        private static readonly string[] _validMimeTypes = ["image/png", "image/jpeg"];
        private const int _fileSizeLimitInMB = 10;

        public override bool IsValid(object? value)
        {
            if (value is null || value is not IFormFile file)
            {
                return false;
            }

            string extension = Path.GetExtension(Path.GetFileName(file.FileName))
                .ToLowerInvariant();

            if (!_validExtensions.Any(ext => ext == extension) || !_validMimeTypes.Any(mime => mime == file.ContentType))
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
