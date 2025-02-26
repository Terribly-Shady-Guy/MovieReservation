using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.EfConfig
{
    internal class ShowingSeatConfig : IEntityTypeConfiguration<ShowingSeat>
    {
        public void Configure(EntityTypeBuilder<ShowingSeat> builder)
        {
            builder.HasKey(e => e.ShowingSeatId)
                .HasName("PK_showing_seat_id");

            builder.Property(e => e.ShowingSeatId)
                .HasColumnName("showing_seat_id");

            builder.Property(e => e.ShowingId)
                .IsRequired()
                .HasColumnName("showing_id");

            builder.Property(e => e.SeatId)
                .HasColumnName("seat_id");

            builder.HasOne(e => e.Showing)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.ShowingId)
                .HasConstraintName("FK_showing_seat_showing");

            builder.HasOne(e => e.Seat)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.SeatId)
                .HasConstraintName("FK_showing_seat_seat");
        }
    }
}
