using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.EfConfig
{
    internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id)
                .HasName("pk_user_id");

            builder.Property(e => e.FirstName)
                .HasColumnType("VARCHAR(50)");

            builder.Property(e => e.LastName)
                .HasColumnType("VARCHAR(50)");

            builder.ToTable(nameof(AppUser) + "s");
        }
    }
}
