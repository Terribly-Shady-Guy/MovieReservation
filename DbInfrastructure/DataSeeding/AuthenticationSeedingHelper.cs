using DbInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    internal class AuthenticationSeedingHelper : IDataSeedingHelper
    {
        private readonly IdentityRole[] _newRoles; 
        private readonly AppUser _seededAdmin;
        private readonly DbContext _context;

        public AuthenticationSeedingHelper(DbContext context)
        {
            _context = context;

            _newRoles = [
                new IdentityRole("User") { NormalizedName = "User".ToUpper() },
                new IdentityRole("Admin") { NormalizedName = "Admin".ToUpper() },
                new IdentityRole("SuperAdmin") { NormalizedName = "SuperAdmin".ToUpper() },
            ];

            var seededAdmin = new AppUser
            {
                UserName = "root",
                AccessFailedCount = 0,
                Email = "root@example.com",
                NormalizedEmail = "root@example.com".ToUpper(),
                EmailConfirmed = true,
                NormalizedUserName = "root".ToUpper(),
                TwoFactorEnabled = false,
                FirstName = "root",
                LastName = "root",
            };

            var hasher = new PasswordHasher<AppUser>();
            seededAdmin.PasswordHash = hasher.HashPassword(seededAdmin, "Admin246810@");

            _seededAdmin = seededAdmin;
        }

        public void Add()
        {
            var roles = _context.Set<IdentityRole>()
                   .ToList();

            foreach (var role in _newRoles)
            {
                if (!roles.Any(r => r.Name == role.Name))
                {
                    _context.Add(role);
                }
            }

            var user = _context.Set<AppUser>()
                .FirstOrDefault(a => a.UserName == "root" && a.Email == "root@example.com");

            if (user is null)
            {
                string adminRoleId = _newRoles.Where(r => r.Name == "SuperAdmin")
                    .Select(r => r.Id)
                    .First();

                var seededAdminRole = new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = _seededAdmin.Id
                };

                _context.Add(_seededAdmin);
                _context.Add(seededAdminRole);
            }
        }

        public async Task AddAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Set<IdentityRole>()
                    .ToListAsync(cancellationToken);

            foreach (var role in _newRoles)
            {
                if (!roles.Any(r => r.Name == role.Name))
                {
                    await _context.AddAsync(role, cancellationToken);
                }
            }

            var user = await _context.Set<AppUser>()
                .FirstOrDefaultAsync(a => a.UserName == "root" && a.Email == "root@example.com", cancellationToken: cancellationToken);

            if (user is null)
            {
                string adminRoleId = _newRoles.Where(r => r.Name == "SuperAdmin")
                    .Select(r => r.Id)
                    .First();

                var seededAdminRole = new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = _seededAdmin.Id
                };

                await _context.AddAsync(seededAdminRole, cancellationToken);
                await _context.AddAsync(_seededAdmin, cancellationToken);
            }
        }
    } 
}
