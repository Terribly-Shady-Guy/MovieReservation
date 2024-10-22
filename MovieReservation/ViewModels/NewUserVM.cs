using System.ComponentModel.DataAnnotations;

namespace MovieReservation.ViewModels
{
    public class NewUserVM
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
