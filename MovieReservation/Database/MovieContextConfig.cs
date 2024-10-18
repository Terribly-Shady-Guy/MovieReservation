using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;

namespace MovieReservation.Database
{
    public static class MovieContextConfig
    {
        public static ModelBuilder AddMovieModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Movie>();

            entity.HasKey(e => e.MovieId)
                .HasName("PK_movie_id");

            entity.Property(e => e.MovieId)
            .HasColumnName("movie_id");

            entity.Property(e => e.Title)
            .HasColumnName("title")
            .IsRequired()
            .IsUnicode(false);

            entity.Property(e => e.PosterImageName)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("poster_image_name");

            entity.Property(e => e.Description)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("description")
            .HasMaxLength(300);

            entity.Property(e => e.Genre)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnName("genre");

            entity.HasIndex(e => e.Genre);
            entity.HasIndex(e => e.Title);

            return modelBuilder;
        }
    }
}
