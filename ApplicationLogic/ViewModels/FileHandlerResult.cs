namespace ApplicationLogic.ViewModels
{
    public class FileHandlerResult
    {
        public bool Success { get; init; }
        public Stream? FileStream { get; init; }
        public string? FileName { get; init; }

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
