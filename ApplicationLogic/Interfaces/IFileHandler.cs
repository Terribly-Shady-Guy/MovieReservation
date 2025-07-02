using ApplicationLogic.ViewModels;

namespace ApplicationLogic.Interfaces
{
    public interface IFileHandler
    {
        Task CreateFile(IFormFile file, string fileName);
        void DeleteFile(string fileName);
        FileHandlerResult GetFile(string fileName);
    }
}