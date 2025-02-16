using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(e => e.LocationId)
                .HasName("PK_location_id");

            builder.Property(e => e.LocationId)
                .HasColumnName("location_id");

            builder.Property(e => e.Street)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("street");

            builder.Property(e => e.City)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("city");

            builder.Property(e => e.State)
                .IsRequired()
                .HasColumnName("state")
                .IsUnicode(false);

            builder.Property(e => e.Zip)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("zip")
                .HasMaxLength(5)
                .IsFixedLength();
        }
    }
}
