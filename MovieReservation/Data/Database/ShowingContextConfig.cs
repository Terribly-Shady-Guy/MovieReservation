using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Data.Database
{
    internal static class ShowingContextConfig
    {
        public static ModelBuilder AddShowingModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Showing>();

            entity.HasKey(e => e.ShowingId)
                .HasName("PK_showing_id");

            entity.Property(e => e.ShowingId)
                .HasColumnName("showing_id");

            entity.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired()
                .HasColumnType("DATETIME");

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("price")
                .HasColumnType("MONEY");

            entity.Property(e => e.MovieId)
                .IsRequired()
                .HasColumnName("movie_id");

            entity.HasOne(e => e.Movie)
                .WithMany(e => e.Showings)
                .HasForeignKey(e => e.MovieId)
                .HasConstraintName("FK_movie_showing");

            entity.ToTable("Showings", schema =>
            {
                schema.HasCheckConstraint("CK_min_price", "price > 0");
            });

            return modelBuilder;
        }
    }
}
