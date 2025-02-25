using Microsoft.AspNetCore.Identity;

namespace DbInfrastructure.Models
{
    public class AppUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
       
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<InternalLogin> UserLogins { get; set; } = new List<InternalLogin>();
    }
}
