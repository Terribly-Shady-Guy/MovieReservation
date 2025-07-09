namespace ApplicationLogic.ViewModels
{
    public class FileHandlerResult
    {
        public bool Success { get; }
        public Stream? FileStream { get; }
        public string? FileName { get; }

        private FileHandlerResult(bool success, Stream? fileStream, string? fileName)
        {
            Success = success;
            FileStream = fileStream;
            FileName = fileName;
        }

        public static FileHandlerResult Succeeded(Stream stream, string filename)
        {
            return new FileHandlerResult(true, stream, filename);
        }

        public static FileHandlerResult Failed()
        {
            return new FileHandlerResult(false, null, null);
        }
    }
}
