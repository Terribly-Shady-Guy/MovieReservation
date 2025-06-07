using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class SeatConfig : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(e => e.SeatId)
                .HasName("PK_Seats");

            builder.Property(e => e.SeatId)
                .HasColumnName("seat_id");

            builder.Property(e => e.AuditoriumNumber)
                .IsRequired(true)
                .HasColumnName("auditorium_number");

            builder.Property(e => e.RowIdentifier)
                .HasColumnName("row_identifier")
                .HasColumnType("CHAR(1)")
                .HasMaxLength(1)
                .IsFixedLength()
                .IsRequired();

            builder.Property(e => e.RowSeatNumber)
                .HasColumnType("INTEGER")
                .HasColumnName("row_seat_number")
                .IsRequired();
            
            builder.HasIndex(e => new { e.AuditoriumNumber, e.RowIdentifier, e.RowSeatNumber })
                .IsUnique()
                .HasDatabaseName("IX_Seat_Row");

            builder.HasOne(e => e.Auditorium)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.AuditoriumNumber)
                .HasConstraintName("FK_Seats_Auditoriums");
        }
    }
}
