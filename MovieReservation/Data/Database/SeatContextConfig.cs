using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class SeatContextConfig
    {
        public static ModelBuilder AddSeatModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Seat>();

            entity.HasKey(e => e.SeatId)
                .HasName("PK_seat_id");

            entity.Property(e => e.SeatId)
            .HasColumnName("seat_id");

            entity.Property(e => e.Price)
            .IsRequired()
            .HasColumnName("price")
            .HasColumnType("MONEY");

            entity.Property(e => e.AuditoriumNumber)
            .IsRequired(true)
            .HasColumnName("auditorium_number");

            entity.HasOne(e => e.Auditorium)
            .WithMany(e => e.Seats)
            .HasForeignKey(e => e.AuditoriumNumber)
            .HasConstraintName("FK_seat_auditorium");

            return modelBuilder;
        }
    }
}
