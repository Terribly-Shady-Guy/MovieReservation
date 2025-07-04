namespace ApplicationLogic.ViewModels
{
    public class FileHandlerResult
    {
        public bool Success { get; private set; }
        public Stream? FileStream { get; private set; }
        public string? FileName { get; private set; }

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
