using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class AuditoriumContextConfig
    {
        public static ModelBuilder AddAuditoriumModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Auditorium>();

            entity.HasKey(e => e.AuditoriumNumber)
                .HasName("PK_auditorium_number");

            entity.Property(e => e.AuditoriumNumber)
                .IsRequired()
                .HasColumnName("auditorium_number")
                .HasColumnType("VARCHAR(10)")
                .IsUnicode(false);

            entity.Property(e => e.MaxCapacity)
                .IsRequired()
                .HasColumnName("max_capacity");

            entity.Property(e => e.LocationId)
                .HasColumnName("location_id")
                .IsRequired();

            entity.HasOne(e => e.Location)
                .WithMany(e => e.Auditoriums)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_auditorium_location");

            entity.ToTable("Auditoriums", table =>
            {
                table.HasCheckConstraint("CK_max_capacity", "max_capacity > 0");
            });

            return modelBuilder;
        }
    }
}
