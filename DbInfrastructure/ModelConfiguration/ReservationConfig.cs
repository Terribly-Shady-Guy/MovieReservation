﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class ReservationConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_Reservations");

            builder.Property(e => e.Id)
                .HasColumnName("reservation_id");

            builder.Property(e => e.DateReserved)
                .IsRequired()
                .HasColumnType("DATETIME")
                .HasColumnName("date_reserved");

           builder.Property(e => e.Status)
                .HasColumnType("INTEGER")
                .IsRequired()
                .HasConversion<int>();
            
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
                .HasConstraintName("FK_AppUsers_Reservations");

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
                        .HasName("PK_ReservedSeats");
                });
        }
    }
}
