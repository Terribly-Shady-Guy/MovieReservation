namespace ApplicationLogic.ViewModels
{
    public class FileHandlerResult
    {
        public required bool Success { get; set; }
        public Stream? FileStream { get; set; }
        public string? FileName { get; set; }
    }
}
