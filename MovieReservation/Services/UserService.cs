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

        public async Task<AppUser?> GetUserAsync(UserLoginVM loginUser)
        {
            List<AppUser> users = await _context.AppUsers.Where(e => e.Username == loginUser.Username).ToListAsync();
            var hasher = new PasswordHasher<AppUser>();

            foreach (var user in users)
            {
                var result = hasher.VerifyHashedPassword(user, user.Password, loginUser.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<AppUser?> GetUserAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task UpdateRefreshToken(string? token, DateTime? expiration, AppUser user)
        {
            user.RefreshToken = token;
            user.ExpirationDate = expiration;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshToken(string? token, DateTime? expiration, int id)
        {
            AppUser? user = await _context.AppUsers.FindAsync(id);
            if (user == null) { return; }

            await UpdateRefreshToken(token, expiration, user);
        }
    }
}
