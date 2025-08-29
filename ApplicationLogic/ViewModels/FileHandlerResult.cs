using System.Diagnostics.CodeAnalysis;

namespace ApplicationLogic.ViewModels
{
    public sealed class FileHandlerResult
    {
        private FileHandlerResult(Stream? fileStream)
        {
            FileStream = fileStream;
        }

        [MemberNotNullWhen(true, nameof(FileStream))]
        public bool IsSuccess => FileStream is not null;
        public Stream? FileStream { get; }

        public static FileHandlerResult Succeeded(Stream stream)
        {
            return new FileHandlerResult(stream);
        }

        public static FileHandlerResult Failed()
        {
            return new FileHandlerResult(null);
        }
    }
}
