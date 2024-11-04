using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class ShowingSeatContextConfig
    {
        public static ModelBuilder AddShowingSeatModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ShowingSeat>();

            entity.HasKey(e => e.ShowingSeatId)
            .HasName("PK_showing_seat_id");

            entity.Property(e => e.ShowingSeatId)
            .HasColumnName("showing_seat_id");

            entity.Property(e => e.ShowingId)
            .IsRequired()
            .HasColumnName("showing_id");

            entity.Property(e => e.SeatId)
            .HasColumnName("seat_id");

            entity.HasOne(e => e.Showing)
            .WithMany(e => e.ShowingSeats)
            .HasForeignKey(e => e.ShowingId)
            .HasConstraintName("FK_showing_seat_showing");

            entity.HasOne(e => e.Seat)
            .WithMany(e => e.ShowingSeats)
            .HasForeignKey(e => e.SeatId)
            .HasConstraintName("FK_showing_seat_seat");

            return modelBuilder;
        }
    }
}
