using DbInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure
{
    internal class DataSeedingHelper
    {
        private readonly IdentityRole[] _newRoles; 
        private readonly AppUser _seededAdmin;
        private readonly ReservationStatusLookup[] _reservationStatuses;
        private readonly TheaterTypeLookup[] _theaterTypes;
        private readonly DbContext _context;

        public DataSeedingHelper(DbContext context)
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

            _reservationStatuses = Enum.GetValues<ReservationStatus>()
                .Select(status => new ReservationStatusLookup(status))
                .ToArray();

            _theaterTypes = Enum.GetValues<TheaterType>()
                .Select(type => new TheaterTypeLookup(type))
                .ToArray();
        }

        public void AddRootUser()
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

        public async Task AddRootUserAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Set<IdentityRole>()
                    .ToListAsync(cancellationToken);

            foreach (var role in _newRoles)
            {
                if (!roles.Any(r => r.Name == role.Name))
                {
                    _context.Add(role);
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

                _context.Add(seededAdminRole);
                _context.Add(_seededAdmin);
            }
        }

        public void AddEnums()
        {
            var statuses = _context.Set<ReservationStatusLookup>()
                   .ToList();

            foreach (var status in _reservationStatuses)
            {
                if (!statuses.Any(s => s.Name == status.Name && s.Id == status.Id))
                {
                    _context.Add(status);
                }
            }

            var types = _context.Set<TheaterTypeLookup>()
                .ToList();

            foreach (var type in _theaterTypes)
            {
                if (!types.Any(t => t.Name == type.Name && t.Id == type.Id))
                {
                    _context.Add(type);
                }
            }
        }

        public async Task AddEnumsAsync(CancellationToken cancellationToken)
        {
            var statuses = await _context.Set<ReservationStatusLookup>()
                  .ToListAsync(cancellationToken);

            foreach (var status in _reservationStatuses)
            {
                if (!statuses.Any(s => s.Name == status.Name && s.Id == status.Id))
                {
                    _context.Add(status);
                }
            }

            var types = await _context.Set<TheaterTypeLookup>()
                .ToListAsync(cancellationToken);

            foreach (var type in _theaterTypes)
            {
                if (!types.Any(t => t.Name == type.Name && t.Id == type.Id))
                {
                    _context.Add(type);
                }
            }
        }
    }
}
