using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.EfConfig
{
    internal class AuditoriumConfig : IEntityTypeConfiguration<Auditorium>
    {
        public void Configure(EntityTypeBuilder<Auditorium> builder)
        {
            builder.HasKey(e => e.AuditoriumNumber)
                .HasName("PK_auditorium_number");

            builder.Property(e => e.AuditoriumNumber)
                .IsRequired()
                .HasColumnName("auditorium_number")
                .HasColumnType("VARCHAR(10)")
                .IsUnicode(false);

            builder.Property(e => e.MaxCapacity)
                .IsRequired()
                .HasColumnName("max_capacity");

            builder.Property(e => e.LocationId)
                .HasColumnName("location_id")
                .IsRequired();

            builder.HasOne(e => e.Location)
                .WithMany(e => e.Auditoriums)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_auditorium_location");

            builder.ToTable("Auditoriums", table =>
            {
                table.HasCheckConstraint("CK_max_capacity", "max_capacity > 0");
            });
        }
    }
}
