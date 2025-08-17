using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class MovieConfig : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_Movies");

            builder.Property(e => e.Id)
                .HasColumnName("movie_id");

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(100);

            builder.Property(e => e.ImageFileName)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("image_file_name")
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("description")
                .HasMaxLength(300);

            builder.HasIndex(e => e.Title);
        }
    }
}
