using System.ComponentModel.DataAnnotations;

namespace MovieReservation.ViewModels
{
    public class NewUserVM
    {
        [MinLength(2)]
        public required string FirstName { get; set; }
        [MinLength(2)]
        public required string LastName { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(1)]
        public required string Username { get; set; }
        [RegularExpression(@"^(?:(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%&*+=]))[\da-zA-Z!@#$%&*+=]{8,}$",
            ErrorMessage = "Password does not contain an upercase letter, lowercase letter, number, or special character, or is not at least 8 characters long.")]
        public required string Password { get; set; }
    }
}
