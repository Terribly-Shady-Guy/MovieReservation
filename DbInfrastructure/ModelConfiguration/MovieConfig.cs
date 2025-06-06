using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class MovieConfig : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(e => e.MovieId)
                .HasName("PK_Movied");

            builder.Property(e => e.MovieId)
                .HasColumnName("movie_id");

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(100);

            builder.Property(e => e.PosterImageName)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("poster_image_name")
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("description")
                .HasMaxLength(300);

            builder.Property(e => e.Genre)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("genre")
                .HasMaxLength(20);

            builder.HasIndex(e => e.Genre);
            builder.HasIndex(e => e.Title);
        }
    }
}
