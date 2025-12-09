namespace ApplicationLogic.ViewModels
{
    public class ResponseMessage
    {
        public required string Message { get; set; }
    }

    public class TwoFactorMessage : ResponseMessage
    {
        public required string UserId { get; set; }
    }
}
