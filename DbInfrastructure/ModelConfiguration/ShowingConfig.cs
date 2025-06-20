﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    internal class ShowingConfig : IEntityTypeConfiguration<Showing>
    {
        public void Configure(EntityTypeBuilder<Showing> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_Showings");

            builder.Property(e => e.Id)
                .HasColumnName("showing_id");

            builder.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired()
                .HasColumnType("DATETIME");

            builder.Property(e => e.MovieId)
                .IsRequired()
                .HasColumnName("movie_id");

            builder.HasOne(e => e.Movie)
                .WithMany(e => e.Showings)
                .HasForeignKey(e => e.MovieId)
                .HasConstraintName("FK_Movies_Showings");
        }
    }
}
