using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static void UseDataSeeding(this DbContextOptionsBuilder options)
        {
            List<IdentityRole> newRoles =
            [
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "User"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "Admin"
                }
            ];

            var hasher = new PasswordHasher<AppUser>();

            AppUser seededAdmin = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "root",
                AccessFailedCount = 0,
                Email = "root@example.com",
                NormalizedEmail = "root@example.com",
                EmailConfirmed = true,
                NormalizedUserName = "root",
                TwoFactorEnabled = false,
                ExpirationDate = null,
                RefreshToken = null,
                FirstName = "root",
                LastName = "root",
            };

            seededAdmin.PasswordHash = hasher.HashPassword(seededAdmin, "admin246810");

            options.UseSeeding((context, _) =>
            {
                var roles = context.Set<IdentityRole>()
                    .Where(r => r.Name == "User" || r.Name == "Admin")
                    .ToList();

                foreach (var role in newRoles)
                {
                    if (!roles.Any(r => r.Name == role.Name))
                    {
                        context.Add(role);
                    }
                }

                var user = context.Set<AppUser>()
                    .FirstOrDefault(a => a.UserName == "root" && a.Email == "root@example.com");

                if (user is null)
                {
                    string adminRoleId = newRoles.Where(r => r.Name == "Admin")
                    .Select(r => r.Id)
                    .First();

                    var seededAdminRole = new IdentityUserRole<string>
                    {
                        RoleId = adminRoleId,
                        UserId = seededAdmin.Id
                    };

                    context.Add(seededAdmin);
                    context.Add(seededAdminRole);
                }

                context.SaveChanges();
            });

            options.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var roles = await context.Set<IdentityRole>()
                    .Where(r => r.Name == "User" || r.Name == "Admin")
                    .ToListAsync(cancellationToken);

                foreach (var role in newRoles)
                {
                    if (!roles.Any(r => r.Name == role.Name))
                    {
                        context.Add(role);
                    }
                }

                var user = await context.Set<AppUser>()
                    .FirstOrDefaultAsync(a => a.UserName == "root" && a.Email == "root@example.com", cancellationToken: cancellationToken);

                if (user is null)
                {
                    string adminRoleId = newRoles.Where(r => r.Name == "Admin")
                    .Select(r => r.Id)
                    .First();

                    var seededAdminRole = new IdentityUserRole<string>
                    {
                        RoleId = adminRoleId,
                        UserId = seededAdmin.Id
                    };

                    context.Add(seededAdminRole);
                    context.Add(seededAdmin);
                }

                await context.SaveChangesAsync(cancellationToken);
            });
        }
    }
}
