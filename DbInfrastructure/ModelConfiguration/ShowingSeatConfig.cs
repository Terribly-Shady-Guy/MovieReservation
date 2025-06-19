using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class ShowingSeatConfig : IEntityTypeConfiguration<ShowingSeat>
    {
        public void Configure(EntityTypeBuilder<ShowingSeat> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_ShowingSeats");

            builder.Property(e => e.Id)
                .HasColumnName("showing_seat_id");

            builder.Property(e => e.ShowingId)
                .IsRequired()
                .HasColumnName("showing_id");
            
            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("price")
                .HasColumnType("MONEY");

            builder.Property(e => e.SeatId)
                .HasColumnName("seat_id");

            builder.HasOne(e => e.Showing)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.ShowingId)
                .HasConstraintName("FK_ShowingSeats_Showings");

            builder.HasOne(e => e.Seat)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.SeatId)
                .HasConstraintName("FK_ShowingSeats_Seats");

            builder.ToTable(nameof(ShowingSeat) + "s", schema =>
            {
                schema.HasCheckConstraint("CK_min_price", "price > 0");
            });
        }
    }
}
