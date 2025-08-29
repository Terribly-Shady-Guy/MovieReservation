using DbInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    internal class AuthenticationDataSeeder : IDataSeeder
    {
        private readonly IdentityRole[] _newRoles; 
        private readonly AppUser _seededAdmin;

        public AuthenticationDataSeeder()
        {
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

        public void Add(DbContext context)
        {
            var roles = context.Set<IdentityRole>()
                   .ToList();

            AddRoles(context, roles);

            var user = context.Set<AppUser>()
                .FirstOrDefault(a => a.UserName == "root" && a.Email == "root@example.com");

            if (user is null)
            {
                AddUser(context);
            }
        }

        public async Task AddAsync(DbContext context, CancellationToken cancellationToken)
        {
            var roles = await context.Set<IdentityRole>()
                    .ToListAsync(cancellationToken);

           AddRoles(context, roles);

            var user = await context.Set<AppUser>()
                .FirstOrDefaultAsync(a => a.UserName == "root" && a.Email == "root@example.com", cancellationToken: cancellationToken);

            if (user is null)
            {
                AddUser(context);
            }
        }

        private void AddRoles(DbContext context, List<IdentityRole> roles)
        {
            foreach (var role in _newRoles)
            {
                if (!roles.Any(r => r.Name == role.Name))
                {
                    context.Add(role);
                }
            }
        }

        private void AddUser(DbContext context)
        {
            string adminRoleId = _newRoles.Where(r => r.Name == "SuperAdmin")
                    .Select(r => r.Id)
                    .First();

            var seededAdminRole = new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = _seededAdmin.Id
            };

            context.Add(_seededAdmin);
            context.Add(seededAdminRole);
        }
    } 
}
