using Microsoft.AspNetCore.Identity;
using DbInfrastructure.Models;
using ApplicationLogic.ViewModels;
using DbInfrastructure;

namespace ApplicationLogic.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MovieReservationDbContext _dbContext;

        public UserService(
            UserManager<AppUser> userManager,
            MovieReservationDbContext dbContext,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<Result<string>> AddNewUserAsync(NewUserDto newUser)
        {
            var user = new AppUser()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                UserName = newUser.Username,
            };

            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var result = await _userManager.CreateAsync(user, newUser.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return Result<string>.Fail("Could not create user.");
                }

                if (!await _roleManager.RoleExistsAsync(Roles.User))
                {
                    await transaction.RollbackAsync();
                    return Result<string>.Fail($"Could not find \"{Roles.User}\"");
                }

                result = await _userManager.AddToRoleAsync(user, Roles.User);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return Result<string>.Fail($"Could not add user to role \"{Roles.User}\"");
                }

                await transaction.CommitAsync();
            }

            string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            Console.WriteLine($"User Id: {user.Id}. Confirmation Token: {confirmationToken}");

            return Result<string>.Success(user.Id);
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

        public async Task<Result> PromoteToAdmin(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result.Fail("Could not find user.");
            }

            if (await _userManager.IsInRoleAsync(user, Roles.Admin))
            {
                return Result.Success();
            }

            if (!await _roleManager.RoleExistsAsync(Roles.User))
            {
                return Result.Fail($"The role {Roles.User} does not exist.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, Roles.User);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result.Fail("Could not remove user's role.");
            }

            if (!await _roleManager.RoleExistsAsync(Roles.Admin))
            {
                await transaction.RollbackAsync();
                return Result.Fail($"The role {Roles.Admin} does not exist.");
            }

            result = await _userManager.AddToRoleAsync(user, Roles.Admin);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result.Fail($"Could not add {Roles.Admin} to this user.");
            }

            await transaction.CommitAsync();
            return Result.Success();
        }
    }
}
