﻿using Microsoft.AspNetCore.Hosting;

namespace ApplicationLogic.Services
{
    public class LocalFileHandler : IFileHandler
    {
        private readonly string _path;

        public LocalFileHandler(IWebHostEnvironment environment)
        {
            string path = Path.Combine(environment.ContentRootPath, "..", "Images");
            _path = Path.GetFullPath(path);
        }

        public async Task CreateFile(IFormFile file, string fileName)
        {
            string trustedPath = Path.Combine(_path, fileName);

            using FileStream stream = File.Create(trustedPath);
            await file.CopyToAsync(stream);
        }

        public void DeleteFile(string fileName)
        {
            string movieImagePath = Path.Combine(_path, fileName);
            if (File.Exists(movieImagePath))
            {
                File.Delete(movieImagePath);
            }
        }

        public string CreateImagePath(string fileName)
        {
            string filePath = Path.Combine(_path, fileName);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            return $"Images/{fileName}";
        }
    }
}
