using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbInfrastructure.ModelConfiguration
{

    internal class ReservationStatusLookupConfig : IEntityTypeConfiguration<ReservationStatusLookup>
    {
        public void Configure(EntityTypeBuilder<ReservationStatusLookup> builder)
        {
            builder.HasAnnotation("Test:DoNotReset", true);

            builder.HasKey(e => e.Id)
                .HasName("PK_ReservationStatus");

            builder.Property(e => e.Id)
                .HasColumnType("INTEGER")
                .HasColumnName("status_id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("VARCHAR(20)")
                .HasColumnName("name");

            builder.HasMany(e => e.Reservations)
                .WithOne(e => e.StatusLookup)
                .HasForeignKey(e => e.Status)
                .HasConstraintName("FK_Reservation_ReservationStatus");

            builder.ToTable("ReservationStatus");
        }
    }

    internal class TheaterTypeLookupConfig : IEntityTypeConfiguration<TheaterTypeLookup>
    {
        public void Configure(EntityTypeBuilder<TheaterTypeLookup> builder)
        {
            builder.HasAnnotation("Test:DoNotReset", true);

            builder.HasKey(e => e.Id)
                .HasName("PK_TheaterType");

            builder.Property(e => e.Id)
                .HasColumnType("INTEGER")
                .HasColumnName("type_id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("VARCHAR(20)")
                .HasColumnName("name");

            builder.HasMany(e => e.Auditoriums)
                .WithOne(e => e.TypeLookup)
                .HasForeignKey(e => e.Type)
                .HasConstraintName("FK_Auditoriums_TheaterType");

            builder.ToTable("TheaterType");
        }
    }
}
