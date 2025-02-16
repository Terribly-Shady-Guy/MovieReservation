using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Data.EfConfig;
using MovieReservation.Models;

namespace MovieReservation.Data.DbContexts
{
    public class MovieReservationDbContext : IdentityDbContext<AppUser>
    {
        // Disabled nullability warnings for DbSet props as they are handled by efcore.
        // Identity doesn't suppress it for some reason.
#pragma warning disable CS8618
        public MovieReservationDbContext(DbContextOptions<MovieReservationDbContext> options) : base(options)
        {

        }
#pragma warning restore CS8618

        public DbSet<InternalLogin> Logins { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showing> Showings { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowingSeat> ShowingSeats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new InternalLoginConfig());
            modelBuilder.ApplyConfiguration(new AppUserConfig());
            modelBuilder.ApplyConfiguration(new AuditoriumConfig());
            modelBuilder.ApplyConfiguration(new LocationConfig());
            modelBuilder.ApplyConfiguration(new MovieConfig());
            modelBuilder.ApplyConfiguration(new ReservationConfig());
            modelBuilder.ApplyConfiguration(new SeatConfig());
            modelBuilder.ApplyConfiguration(new ShowingConfig());
            modelBuilder.ApplyConfiguration(new ShowingSeatConfig());
        }
    }
}
