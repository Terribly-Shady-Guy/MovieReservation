
namespace MovieReservation.Services
{
    public class LocalFileHandler : IFileHandler
    {
        private readonly string _path = Path.Combine(Environment.CurrentDirectory, "..", "Images");

        public async Task CreateFile(IFormFile file)
        {
            string trustedPath = Path.Combine(_path, file.FileName);

            using (var stream = File.Create(trustedPath))
            {
                await file.CopyToAsync(stream);
            }
        }

        public void DeleteFile(string fileName)
        {
            string movieImagePath = Path.Combine(_path, fileName);
            File.Delete(movieImagePath);
        }
    }
}
