using Microsoft.AspNetCore.Identity;
using DbInfrastructure.Models;
using ApplicationLogic.ViewModels;

namespace ApplicationLogic.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> AddNewUserAsync(NewUserVM newUser)
        {
            var user = new AppUser()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                UserName = newUser.Username,
            };

            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (!result.Succeeded)
            {
                return null;
            }

            result = await _userManager.AddToRoleAsync(user, "User");
            if (!result.Succeeded) { return null; }

            return user.Id;
        }

        public async Task ConfirmEmail(string id, string token)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return;
            }

            await _userManager.ConfirmEmailAsync(user, token);
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
