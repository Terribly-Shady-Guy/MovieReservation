namespace DbInfrastructure.Models
{
    public abstract class EnumLookup<TEnum>
        where TEnum : struct, Enum
    {
        protected EnumLookup(TEnum value)
        {
            Id = value;
            Name = value.ToString();
        }

        protected EnumLookup() { }

        public TEnum Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ReservationStatusLookup : EnumLookup<ReservationStatus>
    {
        public ReservationStatusLookup()
        {
        }

        public ReservationStatusLookup(ReservationStatus value) : base(value)
        {
        }

        public ICollection<Reservation> Reservations { get; set; } = [];
    }

    public class TheaterTypeLookup : EnumLookup<TheaterType>
    {
        public TheaterTypeLookup()
        {
        }

        public TheaterTypeLookup(TheaterType value) : base(value)
        {
        }

        public ICollection<Auditorium> Auditoriums { get; set; } = [];
    }
}
