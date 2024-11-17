using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Data.DbContexts;
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

        public async Task<string?> AddNewUserAsync(NewUserVM newUser)
        {
            var hasher = new PasswordHasher<AppUser>();

            var user = new AppUser() 
            { 
                Email = newUser.Email,
                UserName = newUser.Username,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                //Role = "User"
            };

            user.PasswordHash = hasher.HashPassword(user, newUser.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return await _context.Users.Where(e => e.UserName == user.UserName && e.Email == user.Email)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PromoteToAdmin(string id)
        {
            AppUser? user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            //user.Role = "Admin";

            _context.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<AppUser?> GetUserAsync(UserLoginVM loginUser)
        {
            List<AppUser> users = await _context.Users.Where(e => e.UserName == loginUser.Username).ToListAsync();
            var hasher = new PasswordHasher<AppUser>();

            foreach (var user in users)
            {
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<AppUser?> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
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
            AppUser? user = await GetUserAsync(id);
            if (user == null) { return; }

            await UpdateRefreshToken(token, expiration, user);
        }
    }
}
