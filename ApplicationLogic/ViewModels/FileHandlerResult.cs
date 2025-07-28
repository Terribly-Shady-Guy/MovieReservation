namespace ApplicationLogic.ViewModels
{
    public class FileHandlerResult
    {
        public bool Success => FileStream is not null;
        public Stream? FileStream { get; }
        public string? FileName { get; }

        private FileHandlerResult(Stream? fileStream, string? fileName)
        {
            FileStream = fileStream;
            FileName = fileName;
        }

        public static FileHandlerResult Succeeded(Stream stream, string filename)
        {
            return new FileHandlerResult(stream, filename);
        }

        public static FileHandlerResult Failed()
        {
            return new FileHandlerResult(null, null);
        }
    }
}
