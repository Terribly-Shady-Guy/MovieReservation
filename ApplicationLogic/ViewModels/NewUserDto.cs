using System.ComponentModel.DataAnnotations;

namespace ApplicationLogic.ViewModels
{
    public class NewUserDto
    {
        [MinLength(2)]
        public required string FirstName { get; set; }
        [MinLength(2)]
        public required string LastName { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(5)]
        public required string Username { get; set; }
        [RegularExpression(@"^(?:(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%&*+=]))[\da-zA-Z!@#$%&*+=]{8,}$",
            ErrorMessage = "Password does not contain an upercase letter, lowercase letter, number, or special character, or is not at least 8 characters long.")]
        public required string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "The confirming password does not match the supplied one.")]
        public required string ConfirmPassword { get; set; }
    }
}
