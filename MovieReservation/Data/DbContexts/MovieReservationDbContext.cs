using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Data.Database;
using MovieReservation.Models;

namespace MovieReservation.Data.DbContexts
{
    public class MovieReservationDbContext : IdentityDbContext<AppUser>
    {
        //disabled nullability warnings on dbset props as they are handled by efcore.
        //Identity doesn't suppress it for some reason.
#pragma warning disable CS8618
        public MovieReservationDbContext(DbContextOptions<MovieReservationDbContext> options) : base(options)
        {

        }
#pragma warning restore CS8618

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Showing> Showings { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Auditorium> Auditoriums { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<ShowingSeat> ShowingSeats { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
