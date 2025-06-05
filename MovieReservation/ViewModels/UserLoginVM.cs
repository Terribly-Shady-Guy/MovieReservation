using System.ComponentModel.DataAnnotations;

namespace MovieReservation.ViewModels
{
    public class UserLoginVM
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
