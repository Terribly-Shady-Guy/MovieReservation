using System.Diagnostics.CodeAnalysis;

namespace ApplicationLogic.ViewModels
{
    public sealed class FileHandlerResult
    {
        [MemberNotNullWhen(true, nameof(FileStream), nameof(FileName))]
        public bool IsSuccess => FileStream is not null;
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
