using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_Locations");

            builder.Property(e => e.Id)
                .HasColumnName("location_id");

            builder.Property(e => e.Street)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("street")
                .HasMaxLength(300);

            builder.Property(e => e.City)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("city")
                .HasMaxLength(100);

            builder.Property(e => e.State)
                .IsRequired()
                .HasColumnName("state")
                .IsUnicode(false)
                .HasMaxLength(50);

            builder.Property(e => e.Zip)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("zip")
                .HasMaxLength(5)
                .IsFixedLength();
        }
    }
}
