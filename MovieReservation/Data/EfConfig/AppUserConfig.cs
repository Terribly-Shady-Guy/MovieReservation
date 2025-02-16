using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Models;

namespace MovieReservation.Data.EfConfig
{
    internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id)
                .HasName("pk_user_id");

            builder.ToTable(nameof(AppUser) + "s");
        }
    }
}
