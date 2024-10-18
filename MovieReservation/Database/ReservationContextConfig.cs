using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Database
{
    public static class ReservationContextConfig
    {
        public static ModelBuilder AddReservationModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Reservation>();

            entity.HasKey(e => e.ReservationId)
                .HasName("PK_reservation_id");

            entity.Property(e => e.ReservationId)
            .HasColumnName("reservation_id");

            entity.Property(e => e.DateReserved)
            .IsRequired()
            .HasColumnType("DATETIME")
            .HasColumnName("date_reserved");

            entity.Property(e => e.Total)
            .IsRequired()
            .HasColumnName("total")
            .HasColumnType("MONEY");

            entity.Property(e => e.DateCancelled)
            .IsRequired(false)
            .HasColumnType("DATETIME")
            .HasColumnName("date_cancelled");

            entity.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

            entity.HasOne(e => e.User)
            .WithMany(e => e.Reservations)
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_app_user_reservation");

            entity.HasMany(e => e.ShowingSeats)
            .WithMany(e => e.Reservations)
            .UsingEntity("ReservedSeats", joinEntity =>
            {
                joinEntity.Property<int>("ShowingSeatId")
                .HasColumnName("showing_seat_id")
                .HasColumnType("INTEGER");

                joinEntity.Property<int>("ReservationId")
                .HasColumnName("reservation_id")
                .HasColumnType("INTEGER");

                joinEntity.HasKey("ShowingSeatId", "ReservationId")
                .HasName("PK_showingseat_reservation");
            });

            return modelBuilder;
        }
    }
}
