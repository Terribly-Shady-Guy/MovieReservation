using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservation.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "AppUsers",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "AppUsers",
                type: "varchar(20)",
                unicode: false,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "AppUsers");
        }
    }
}
