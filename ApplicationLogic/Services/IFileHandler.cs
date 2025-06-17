namespace ApplicationLogic.Services
{
    public interface IFileHandler
    {
        Task CreateFile(IFormFile file, string fileName);
        string CreateImagePath(string fileName);
        void DeleteFile(string fileName);
    }
}