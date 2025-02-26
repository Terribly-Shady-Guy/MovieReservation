namespace MovieReservation.ViewModels
{
    public class AuthenticationToken
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime RefreshExpiration { get; set; }
    }
}
