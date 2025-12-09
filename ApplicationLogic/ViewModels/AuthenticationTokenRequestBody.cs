namespace ApplicationLogic.ViewModels
{
    public class AuthenticationTokenRequestBody
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
