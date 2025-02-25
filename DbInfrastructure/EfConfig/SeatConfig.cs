using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.Data.EfConfig
{
    internal class SeatConfig : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(e => e.SeatId)
                .HasName("PK_seat_id");

            builder.Property(e => e.SeatId)
                .HasColumnName("seat_id");

            builder.Property(e => e.AuditoriumNumber)
                .IsRequired(true)
                .HasColumnName("auditorium_number");

            builder.HasOne(e => e.Auditorium)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.AuditoriumNumber)
                .HasConstraintName("FK_seat_auditorium");
        }
    }
}
