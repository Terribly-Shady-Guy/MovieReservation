namespace DbInfrastructure.ViewModels
{
    public class AuthenticationTokenRequestBody
    {
        public string AccessToken { get; set; }
        public  string RefreshToken { get; set; }
    }
}
