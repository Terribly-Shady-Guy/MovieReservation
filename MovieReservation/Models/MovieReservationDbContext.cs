using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MovieReservation.Models
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
            var dataSeeder = new DataSeeder();

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                .HasName("PK_user_id");

                entity.Property(e => e.UserId)
                .HasColumnName("user_id");

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

                entity.Property(e => e.RefreshToken)
                .IsUnicode(false)
                .HasColumnName("refresh_token")
                .HasColumnType("varchar(200)");

                entity.Property(e => e.ExpirationDate)
                .HasColumnType("DATETIME")
                .HasColumnName("expiration_date");

                entity.ToTable(nameof(AppUser) + "s");

                entity.HasData(dataSeeder.SeededAdmin);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId)
                .HasName("PK_reservation_id");

                entity.Property(e => e.ReservationId)
                .HasColumnName("reservation_id");

                entity.Property(e => e.DateReserved)
                .IsRequired()
                .HasColumnType("DATETIME")
                .HasColumnName("date_reserved");

                entity.Property(e => e.Total)
                .IsRequired()
                .HasColumnName("total")
                .HasColumnType("MONEY");

                entity.Property(e => e.DateCancelled)
                .IsRequired(false)
                .HasColumnType("DATETIME")
                .HasColumnName("date_cancelled");

                entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");

                entity.HasOne(e => e.User)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_app_user_reservation");

                entity.HasMany(e => e.ShowingSeats)
                .WithMany(e => e.Reservations)
                .UsingEntity("ReservedSeats", joinEntity =>
                {
                    joinEntity.Property<int>("ShowingSeatId")
                    .HasColumnName("showing_seat_id")
                    .HasColumnType("INTEGER");

                    joinEntity.Property<int>("ReservationId")
                    .HasColumnName("reservation_id")
                    .HasColumnType("INTEGER");

                    joinEntity.HasKey("ShowingSeatId", "ReservationId")
                    .HasName("PK_showingseat_reservation");
                });
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.MovieId)
                .HasName("PK_movie_id");

                entity.Property(e => e.MovieId)
                .HasColumnName("movie_id");

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

                entity.HasIndex(e => e.Genre);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<Showing>(entity =>
            {
                entity.HasKey(e => e.ShowingId)
                .HasName("PK_showing_id");

                entity.Property(e => e.ShowingId)
                .HasColumnName("showing_id");

                entity.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired()
                .HasColumnType("DATETIME");

                entity.Property(e => e.MovieId)
                .IsRequired()
                .HasColumnName("movie_id");

                entity.HasOne(e => e.Movie)
                .WithMany(e => e.Showings)
                .HasForeignKey(e => e.MovieId)
                .HasConstraintName("FK_movie_showing");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocationId)
                .HasName("PK_location_id");

                entity.Property(e => e.LocationId)
                .HasColumnName("location_id");

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
                entity.HasKey(e => e.AuditoriumNumber)
                .HasName("PK_auditorium_number");

                entity.Property(e => e.AuditoriumNumber)
                .IsRequired()
                .HasColumnName("auditorium_number")
                .HasColumnType("VARCHAR(10)")
                .IsUnicode(false);

                entity.Property(e => e.MaxCapacity)
                .IsRequired()
                .HasColumnName("max_capacity");

                entity.Property(e => e.LocationId)
                .HasColumnName("location_id")
                .IsRequired();

                entity.HasOne(e => e.Location)
                .WithMany(e => e.Auditoriums)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_auditorium_location");

                entity.ToTable("Auditoriums", table =>
                {
                    table.HasCheckConstraint("CK_max_capacity", "max_capacity > 0");
                });
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.SeatId)
                .HasName("PK_seat_id");

                entity.Property(e => e.SeatId)
                .HasColumnName("seat_id");

                entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("price")
                .HasColumnType("MONEY");

                entity.Property(e => e.AuditoriumNumber)
                .IsRequired(true)
                .HasColumnName("auditorium_number");

                entity.HasOne(e => e.Auditorium)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.AuditoriumNumber)
                .HasConstraintName("FK_seat_auditorium");
            });

            modelBuilder.Entity<ShowingSeat>(entity =>
            {
                entity.HasKey(e => e.ShowingSeatId)
                .HasName("PK_showing_seat_id");

                entity.Property(e => e.ShowingSeatId)
                .HasColumnName("showing_seat_id");

                entity.Property(e => e.ShowingId)
                .IsRequired()
                .HasColumnName("showing_id");

                entity.Property(e => e.SeatId)
                .HasColumnName("seat_id");

                entity.HasOne(e => e.Showing)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.ShowingId)
                .HasConstraintName("FK_showing_seat_showing");

                entity.HasOne(e => e.Seat)
                .WithMany(e => e.ShowingSeats)
                .HasForeignKey(e => e.SeatId)
                .HasConstraintName("FK_showing_seat_seat");
            });
        }

        private class DataSeeder
        {
            public AppUser SeededAdmin { get; private set; }

            public DataSeeder()
            {
                SeededAdmin = new AppUser
                {
                    UserId = 1,
                    Email = "someEmail@localhost.com",
                    Username = "root",
                    Password = "admin246810",
                    FirstName = "root",
                    LastName = "root",
                    Role = "Admin"
                };

                var hasher = new PasswordHasher<AppUser>();

                SeededAdmin.Password = hasher.HashPassword(SeededAdmin, SeededAdmin.Password);
            }
        }
    }
}
