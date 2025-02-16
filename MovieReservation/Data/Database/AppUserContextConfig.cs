using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class AppUserContextConfig
    {
        public static ModelBuilder AddAppUserModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AppUser>();

            entity.HasKey(x => x.Id)
                .HasName("pk_user_id");

            entity.Property(e => e.RefreshToken)
                .IsUnicode(false)
                .HasColumnName("refresh_token")
                .HasColumnType("varchar(400)");

            entity.Property(e => e.ExpirationDate)
                .HasColumnType("DATETIME")
                .HasColumnName("expiration_date");

            entity.ToTable(nameof(AppUser) + "s");

            return modelBuilder;
        }
    }
}
