using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbInfrastructure.Models;

namespace DbInfrastructure.ModelConfiguration
{
    public class InternalLoginConfig : IEntityTypeConfiguration<InternalLogin>
    {
        public void Configure(EntityTypeBuilder<InternalLogin> builder)
        {
            builder.HasKey(e => e.LoginId)
                .HasName("PK_Login");

            builder.Property(e => e.RefreshToken)
                .IsUnicode(false)
                .HasColumnName("refresh_token")
                .HasColumnType("varchar(400)");

            builder.Property(e => e.ExpirationDate)
                .HasColumnType("DATETIME")
                .HasColumnName("expiration_date");

            builder.Property(e => e.UserId)
                .IsUnicode(false)
                .HasColumnType("NVARCHAR(450)");

            builder.Property(e => e.LoginDate)
                .HasColumnType("DATETIME");

            builder.Property(e => e.LoginId)
                .IsUnicode(false)
                .HasColumnType("VARCHAR(450)");

            builder.HasOne(e => e.LoggedInUser)
                .WithMany(e => e.UserLogins)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_AppUser_Logins");

            builder.HasIndex(e => e.RefreshToken);
        }
    }
}
