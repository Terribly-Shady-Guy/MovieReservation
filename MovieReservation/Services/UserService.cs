using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;
using MovieReservation.ViewModels;

namespace MovieReservation.Services
{
    public class UserService
    {
        private readonly MovieReservationDbContext _context;

        public UserService(MovieReservationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddNewUserAsync(NewUserVM newUser)
        {
            var hasher = new PasswordHasher<AppUser>();

            var user = new AppUser() 
            { 
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Username = newUser.Username,
                Password = newUser.Password,
                Role = "User"
            };

            user.Password = hasher.HashPassword(user, user.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return await _context.AppUsers.Where(e => e.Username == user.Username && e.Email == user.Email)
                .Select(e => e.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PromoteToAdmin(int id)
        {
            AppUser? user = await _context.AppUsers.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            user.Role = "Admin";

            _context.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
