using Microsoft.EntityFrameworkCore;

namespace MovieReservation.Models
{
    public class MovieReservationDbContext : DbContext 
    {
        public MovieReservationDbContext(DbContextOptions<MovieReservationDbContext> options) : base(options) { }

        public virtual DbSet<AppUser> appUsers { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Showing> Showings { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Auditorium> Auditoriums { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<ShowingSeat> ShowingSeats { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("pk_user_id");

                entity.Property(e => e.Username)
                .IsRequired()
                .HasColumnName("username")
                .HasMaxLength(20);

                entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("email");

                entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                .IsRequired()
                .HasColumnName("last_name");

                entity.Property(e => e.Password)
                .IsRequired()
                .HasColumnName("password");

                entity.Property(e => e.Role)
                .IsRequired()
                .HasColumnName("role");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId).HasName("pk_reservation_id");

                entity.Property(e => e.DateReserved)
                .IsRequired()
                .HasColumnType("DateTime")
                .HasColumnName("date_reserved");

                entity.Property(e => e.Total)
                .IsRequired()
                .HasColumnName("total")
                .HasColumnType("MONEY");

                entity.Property(e => e.DateCancelled)
                .IsRequired(false)
                .HasColumnType("DateTime")
                .HasColumnName("date_cancelled");

                entity.HasOne(e => e.User)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("fk_app_user_reservation");

                entity.HasMany(e => e.ShowingSeats)
                .WithMany(e => e.Reservations)
                .UsingEntity("ReservedSeats");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.MovieId).HasName("pk_movie_id");

                entity.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired()
                .IsUnicode(false);

                entity.Property(e => e.PosterImageName)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("poster_image_name");

                entity.Property(e => e.Description)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("description")
                .HasMaxLength(300);

                entity.Property(e => e.Genre)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("genre");
            });

            modelBuilder.Entity<Showing>(entity =>
            {
                entity.HasKey(e => e.ShowingId).HasName("pk_showing_id");

                entity.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired()
                .HasColumnType("DateTime");

                entity.HasOne(e => e.Movie)
                .WithMany(e => e.Showings)
                .HasForeignKey(e => e.MovieId)
                .HasConstraintName("fk_movie_showing");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocationId).HasName("pk_location_id");

                entity.Property(e => e.Street)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("street");

                entity.Property(e => e.City)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("city");

                entity.Property(e => e.State)
                .IsRequired()
                .HasColumnName("state")
                .IsUnicode(false);

                entity.Property(e => e.Zip)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("zip")
                .HasMaxLength(5)
                .IsFixedLength();
            });

            modelBuilder.Entity<Auditorium>(entity =>
            {
                entity.HasKey(e => e.AuditoriumNumber).HasName("pk_auditorium_number");

                entity.Property(e => e.AuditoriumNumber)
                .IsRequired()
                .HasColumnName("auditorium_number")
                .HasColumnType("VARCHAR(10)")
                .IsUnicode(false);

                entity.Property(e => e.MaxCapacity)
                .IsRequired()
                .HasColumnName("max_capacity");

                entity.HasOne(e => e.Location)
                .WithMany(e => e.Auditoriums)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("fk_auditorium_location");

                entity.ToTable("Auditoriums", table =>
                {
                    table.HasCheckConstraint("chk_max_capacity", "max_capacity > 0");
                });
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.SeatId).HasName("pk_seat_id");

                entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("price")
                .HasColumnType("MONEY");

                entity.HasOne(e => e.Auditorium)
                .WithMany(e => e.Seats)
                .HasConstraintName("fk_seat_auditorium");
            });

            modelBuilder.Entity<ShowingSeat>(entity =>
            {
                entity.HasKey(e => e.ShowingSeatId).HasName("pk_showing_seat_id");

                entity.HasOne(e => e.Showing)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.ShowingId)
                .HasConstraintName("fk_showing_seat_showing");

                entity.HasOne(e => e.Seat)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.SeatId)
                .HasConstraintName("fk_showing_seat_seat");
            });
        }
    }
}
