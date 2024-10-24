using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Infrastructure.Models;

namespace MovieReservation.Data.Database
{
    internal static class AppUserContextConfig
    {
        public static ModelBuilder AddAppUserModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AppUser>();

            entity.HasKey(e => e.UserId)
                .HasName("PK_user_id");

            entity.Property(e => e.UserId)
            .HasColumnName("user_id");

            entity.Property(e => e.Username)
            .IsRequired()
            .HasColumnName("username")
            .HasMaxLength(20);

            entity.Property(e => e.Email)
            .IsRequired()
            .HasColumnName("email");

            entity.Property(e => e.FirstName)
            .IsRequired()
            .HasColumnName("first_name");

            entity.Property(e => e.LastName)
            .IsRequired()
            .HasColumnName("last_name");

            entity.Property(e => e.Password)
            .IsRequired()
            .HasColumnName("password");

            entity.Property(e => e.Role)
            .IsRequired()
            .HasColumnName("role");

            entity.Property(e => e.RefreshToken)
            .IsUnicode(false)
            .HasColumnName("refresh_token")
            .HasColumnType("varchar(200)");

            entity.Property(e => e.ExpirationDate)
            .HasColumnType("DATETIME")
            .HasColumnName("expiration_date");

            entity.ToTable(nameof(AppUser) + "s");

            var seededAdmin = new AppUser
            {
                UserId = 1,
                Email = "someEmail@localhost.com",
                Username = "root",
                Password = "admin246810",
                FirstName = "root",
                LastName = "root",
                Role = "Admin"
            };

            var hasher = new PasswordHasher<AppUser>();

            seededAdmin.Password = hasher.HashPassword(seededAdmin, seededAdmin.Password);

            entity.HasData(seededAdmin);

            return modelBuilder;
        }
    }
}
