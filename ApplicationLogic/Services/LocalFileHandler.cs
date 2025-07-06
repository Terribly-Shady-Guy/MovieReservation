using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services
{
    public class LocalFileHandler : IFileHandler
    {
        private readonly string _path;
        private readonly ILogger<LocalFileHandler> _logger;

        public LocalFileHandler(IWebHostEnvironment environment, ILogger<LocalFileHandler> logger)
        {
            string path = Path.Combine(environment.ContentRootPath, "..", "Images");
            _path = Path.GetFullPath(path);
            _logger = logger;
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

        public FileHandlerResult GetFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            string filePath = Path.Combine(_path, fileName);

            if (!File.Exists(filePath))
            {
                return FileHandlerResult.Failed();
            }

            try
            {
                FileStream stream = File.OpenRead(filePath);
                return FileHandlerResult.Succeeded(stream, fileName);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is IOException)
                {
                    _logger.LogWarning(ex, "Exception handled but logged for possible issue. file name: {FileName} at {FilePath}", fileName, _path);
                    return FileHandlerResult.Failed();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
