
namespace MovieReservation.Services
{
    public interface IFileHandler
    {
        Task CreateFile(IFormFile file);
        void DeleteFile(string fileName);
    }
}