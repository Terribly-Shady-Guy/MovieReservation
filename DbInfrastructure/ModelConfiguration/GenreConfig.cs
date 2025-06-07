using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbInfrastructure.ModelConfiguration
{
    internal class GenreConfig : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(x => x.Id)
                .HasName("PK_Genre");

            builder.Property(x => x.Id)
                .HasColumnName("genre_id");

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(20)");

            builder.HasIndex(e => e.Name);

            builder.HasMany(e => e.Movies)
                .WithMany(e => e.Genres)
                .UsingEntity("MovieGenre", joinEntity =>
                {
                    joinEntity.Property<int>("MovieId")
                        .HasColumnName("movie_id")
                        .HasColumnType("INTEGER");

                    joinEntity.Property<int>("GenreId")
                        .HasColumnName("genre_id")
                        .HasColumnType("INTEGER");

                    joinEntity.HasKey("MovieId", "GenreId")
                        .HasName("PK_MovieGenre");
                });
        }
    }
}
