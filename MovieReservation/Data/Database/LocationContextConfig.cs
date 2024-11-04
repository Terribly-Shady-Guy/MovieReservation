using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class LocationContextConfig
    {
        public static ModelBuilder AddLocationModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Location>();

            entity.HasKey(e => e.LocationId)
                .HasName("PK_location_id");

            entity.Property(e => e.LocationId)
            .HasColumnName("location_id");

            entity.Property(e => e.Street)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("street");

            entity.Property(e => e.City)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("city");

            entity.Property(e => e.State)
            .IsRequired()
            .HasColumnName("state")
            .IsUnicode(false);

            entity.Property(e => e.Zip)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("zip")
            .HasMaxLength(5)
            .IsFixedLength();

            return modelBuilder;
        }
    }
}
