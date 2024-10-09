using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_id", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    street = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    city = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    state = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    zip = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location_id", x => x.location_id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    description = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    poster_image_name = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    genre = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movie_id", x => x.movie_id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    date_reserved = table.Column<DateTime>(type: "DateTime", nullable: false),
                    total = table.Column<decimal>(type: "MONEY", nullable: false),
                    date_cancelled = table.Column<DateTime>(type: "DateTime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_id", x => x.reservation_id);
                    table.ForeignKey(
                        name: "fk_app_user_reservation",
                        column: x => x.user_id,
                        principalTable: "AppUsers",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auditoriums",
                columns: table => new
                {
                    auditorium_number = table.Column<string>(type: "VARCHAR(10)", unicode: false, nullable: false),
                    max_capacity = table.Column<int>(type: "int", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditorium_number", x => x.auditorium_number);
                    table.CheckConstraint("chk_max_capacity", "max_capacity > 0");
                    table.ForeignKey(
                        name: "fk_auditorium_location",
                        column: x => x.location_id,
                        principalTable: "Locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Showings",
                columns: table => new
                {
                    showing_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "DateTime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_showing_id", x => x.showing_id);
                    table.ForeignKey(
                        name: "fk_movie_showing",
                        column: x => x.movie_id,
                        principalTable: "Movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    seat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auditorium_number = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    price = table.Column<decimal>(type: "MONEY", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seat_id", x => x.seat_id);
                    table.ForeignKey(
                        name: "fk_seat_auditorium",
                        column: x => x.auditorium_number,
                        principalTable: "Auditoriums",
                        principalColumn: "auditorium_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowingSeats",
                columns: table => new
                {
                    showing_seat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showing_id = table.Column<int>(type: "int", nullable: false),
                    seat_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_showing_seat_id", x => x.showing_seat_id);
                    table.ForeignKey(
                        name: "fk_showing_seat_seat",
                        column: x => x.seat_id,
                        principalTable: "Seats",
                        principalColumn: "seat_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_showing_seat_showing",
                        column: x => x.showing_id,
                        principalTable: "Showings",
                        principalColumn: "showing_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservedSeats",
                columns: table => new
                {
                    showing_seat_id = table.Column<int>(type: "Integer", nullable: false),
                    reservation_id = table.Column<int>(type: "Integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_showingseat_reservation", x => new { x.showing_seat_id, x.reservation_id });
                    table.ForeignKey(
                        name: "FK_ReservedSeats_Reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "Reservations",
                        principalColumn: "reservation_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservedSeats_ShowingSeats_showing_seat_id",
                        column: x => x.showing_seat_id,
                        principalTable: "ShowingSeats",
                        principalColumn: "showing_seat_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoriums_location_id",
                table: "Auditoriums",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_genre",
                table: "Movies",
                column: "genre");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_user_id",
                table: "Reservations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReservedSeats_reservation_id",
                table: "ReservedSeats",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_auditorium_number",
                table: "Seats",
                column: "auditorium_number");

            migrationBuilder.CreateIndex(
                name: "IX_Showings_movie_id",
                table: "Showings",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShowingSeats_seat_id",
                table: "ShowingSeats",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShowingSeats_showing_id",
                table: "ShowingSeats",
                column: "showing_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedSeats");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ShowingSeats");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Showings");

            migrationBuilder.DropTable(
                name: "Auditoriums");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
