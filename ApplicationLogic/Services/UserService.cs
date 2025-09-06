using Microsoft.AspNetCore.Identity;
using DbInfrastructure.Models;
using ApplicationLogic.ViewModels;
using DbInfrastructure;

namespace ApplicationLogic.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MovieReservationDbContext _dbContext;

        public UserService(UserManager<AppUser> userManager, MovieReservationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<string?> AddNewUserAsync(NewUserDto newUser)
        {
            var user = new AppUser()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                UserName = newUser.Username,
            };

            using var transaction = _dbContext.Database.BeginTransaction();

            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (!result.Succeeded)
            {
                return null;
            }

            result = await _userManager.AddToRoleAsync(user, "User");
            if (!result.Succeeded) { return null; }

            await transaction.CommitAsync();

            return user.Id;
        }

        public async Task<bool> ConfirmEmail(string id, string token)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return false;
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> PromoteToAdmin(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            await _userManager.RemoveFromRoleAsync(user, "User");
            IdentityResult result = await _userManager.AddToRoleAsync(user, "Admin");

            return result.Succeeded;
        }
    }
}
