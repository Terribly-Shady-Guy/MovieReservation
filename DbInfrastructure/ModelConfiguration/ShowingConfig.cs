using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class ShowingConfig : IEntityTypeConfiguration<Showing>
    {
        public void Configure(EntityTypeBuilder<Showing> builder)
        {
            builder.HasKey(e => e.ShowingId)
                .HasName("PK_Showings");

            builder.Property(e => e.ShowingId)
                .HasColumnName("showing_id");

            builder.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired()
                .HasColumnType("DATETIME");

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("price")
                .HasColumnType("MONEY");

            builder.Property(e => e.MovieId)
                .IsRequired()
                .HasColumnName("movie_id");

            builder.HasOne(e => e.Movie)
                .WithMany(e => e.Showings)
                .HasForeignKey(e => e.MovieId)
                .HasConstraintName("FK_Movies_Showings");

            builder.ToTable(nameof(Showing) + "s", schema =>
            {
                schema.HasCheckConstraint("CK_min_price", "price > 0");
            });
        }
    }
}
