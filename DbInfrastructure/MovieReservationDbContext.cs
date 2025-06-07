using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DbInfrastructure.Models;

namespace DbInfrastructure
{
    /// <summary>
    /// DbContext class for interacting with the movie reservation db.
    /// </summary>
    public class MovieReservationDbContext : IdentityDbContext<AppUser>
    {
        public MovieReservationDbContext(DbContextOptions<MovieReservationDbContext> options) 
            : base(options) { }

        public DbSet<InternalLogin> Logins { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showing> Showings { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowingSeat> ShowingSeats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationStatusLookup> ReservationStatuses { get; set; }
        public DbSet<TheaterTypeLookup> TheaterTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Type dbContextType = typeof(MovieReservationDbContext);
            modelBuilder.ApplyConfigurationsFromAssembly(dbContextType.Assembly);
        }
    }
}
