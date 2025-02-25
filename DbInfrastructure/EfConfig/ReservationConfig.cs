using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Models;

namespace MovieReservation.Data.EfConfig
{
    internal class ReservationConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(e => e.ReservationId)
                .HasName("PK_reservation_id");

            builder.Property(e => e.ReservationId)
                .HasColumnName("reservation_id");

            builder.Property(e => e.DateReserved)
                .IsRequired()
                .HasColumnType("DATETIME")
                .HasColumnName("date_reserved");

            builder.Property(e => e.Total)
                .IsRequired()
                .HasColumnName("total")
                .HasColumnType("MONEY");

            builder.Property(e => e.DateCancelled)
                .IsRequired(false)
                .HasColumnType("DATETIME")
                .HasColumnName("date_cancelled");

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            builder.HasOne(e => e.User)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_app_user_reservation");

            builder.HasMany(e => e.ShowingSeats)
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
        }
    }
}
