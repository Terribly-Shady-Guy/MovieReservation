using Microsoft.EntityFrameworkCore;
using MovieReservation.Data.Database;
using MovieReservation.Models;

namespace MovieReservation.Data.DbContexts
{
    public class MovieReservationDbContext : DbContext
    {
        public MovieReservationDbContext(DbContextOptions<MovieReservationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Showing> Showings { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Auditorium> Auditoriums { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<ShowingSeat> ShowingSeats { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAppUserModel()
                .AddReservationModel()
                .AddMovieModel()
                .AddShowingModel()
                .AddLocationModel()
                .AddAuditoriumModel()
                .AddSeatModel()
                .AddShowingSeatModel();
        }
    }
}
