using Microsoft.AspNetCore.Identity;
using MovieReservation.Models;
using MovieReservation.ViewModels;

namespace MovieReservation.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<bool> PromoteToAdmin(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            IdentityResult result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
