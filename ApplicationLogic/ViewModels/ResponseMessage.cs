namespace ApplicationLogic.ViewModels
{
    public class ResponseMessage
    {
        public string Message { get; set; }
    }

    public class TwoFactorMessage : ResponseMessage
    {
        public string UserId { get; set; }
    }
}
