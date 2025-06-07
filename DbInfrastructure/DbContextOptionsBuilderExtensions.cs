using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DbInfrastructure.Models;

namespace DbInfrastructure
{
    internal static class DbContextOptionsBuilderExtensions
    {
        internal static void UseDataSeeding(this DbContextOptionsBuilder options)
        {
            List<IdentityRole> newRoles =
            [
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            ];

            var hasher = new PasswordHasher<AppUser>();

            var seededAdmin = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
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

            seededAdmin.PasswordHash = hasher.HashPassword(seededAdmin, "admin246810");

            ReservationStatusLookup[] reservationStatuses = Enum.GetValues<ReservationStatus>()
                .Select(status => new ReservationStatusLookup(status))
                .ToArray();

            TheaterTypeLookup[] theaterTypes = Enum.GetValues<TheaterType>()
                .Select(type =>  new TheaterTypeLookup(type))
                .ToArray();

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

                var statuses = context.Set<ReservationStatusLookup>()
                    .ToList();

                foreach (var status in reservationStatuses)
                {
                    if (!statuses.Any(s => s.Name == status.Name))
                    {
                        context.Add(status);
                    }
                }

                var types = context.Set<TheaterTypeLookup>()
                    .ToList();

                foreach (var type in theaterTypes)
                {
                    if (!types.Any(t => t.Name == type.Name))
                    {
                        context.Add(type);
                    }
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

                var statuses = await context.Set<ReservationStatusLookup>()
                    .ToListAsync(cancellationToken);

                foreach (var status in reservationStatuses)
                {
                    if (!statuses.Any(s => s.Name == status.Name))
                    {
                        context.Add(status);
                    }
                }

                var types = await context.Set<TheaterTypeLookup>()
                    .ToListAsync(cancellationToken);

                foreach (var type in theaterTypes)
                {
                    if (!types.Any(t => t.Name == type.Name))
                    {
                        context.Add(type);
                    }
                }

                await context.SaveChangesAsync(cancellationToken);
            });
        }
    }
}
