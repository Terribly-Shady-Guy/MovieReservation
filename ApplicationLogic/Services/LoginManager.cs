using ApplicationLogic.ViewModels;
using DbInfrastructure;
using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLogic.Services
{
    public class LoginManager
    {
        private readonly MovieReservationDbContext _dbContext;
        public LoginManager(MovieReservationDbContext dbContext)
        {
               _dbContext = dbContext;
        }

        public async Task CreateLogin(AppUser user, AuthenticationToken token)
        {
            var newLogin = new InternalLogin
            {
                Id = Guid.NewGuid().ToString(),
                RefreshToken = token.RefreshToken,
                ExpirationDate = token.RefreshExpiration,
                UserId = user.Id,
                LoggedInUser = user
            };

            _dbContext.Logins.Add(newLogin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<InternalLogin?> GetLoginSession(AppUser user, string refreshToken)
        {
            return await _dbContext.Logins
                .Where(l => l.UserId == user.Id && l.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateTokens(InternalLogin login, AuthenticationToken token)
        {
            login.RefreshToken = token.RefreshToken;
            login.ExpirationDate = token.RefreshExpiration;

            _dbContext.Logins
                .Entry(login)
                .Property(l => l.RefreshToken)
                .IsModified = true;

            _dbContext.Logins
                .Entry(login)
                .Property(l => l.ExpirationDate)
                .IsModified = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLogin(InternalLogin login)
        {
            _dbContext.Logins.Remove(login);
            await _dbContext.SaveChangesAsync();
        }
    }
}
