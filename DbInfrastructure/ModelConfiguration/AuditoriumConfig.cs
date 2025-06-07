using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class AuditoriumConfig : IEntityTypeConfiguration<Auditorium>
    {
        public void Configure(EntityTypeBuilder<Auditorium> builder)
        {
            builder.HasKey(e => e.AuditoriumNumber)
                .HasName("PK_Auditoriums");

            builder.Property(e => e.AuditoriumNumber)
                .IsRequired()
                .HasColumnName("auditorium_number")
                .HasColumnType("VARCHAR(10)")
                .IsUnicode(false);

            builder.Property(e => e.Type)
                .HasColumnType("INTEGER")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.LocationId)
                .HasColumnName("location_id")
                .IsRequired();

            builder.HasOne(e => e.Location)
                .WithMany(e => e.Auditoriums)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_Auditoriums_Locations");
        }
    }
}
