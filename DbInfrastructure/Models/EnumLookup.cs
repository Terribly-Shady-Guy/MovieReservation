namespace DbInfrastructure.Models
{
    public abstract class EnumLookupBase<TEnum>
        where TEnum : struct, Enum
    {
        protected EnumLookupBase(TEnum value)
        {
            Id = value;
            Name = value.ToString();
        }

        protected EnumLookupBase() { }

        public TEnum Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ReservationStatusLookup : EnumLookupBase<ReservationStatus>
    {
        public ReservationStatusLookup()
        {
        }

        public ReservationStatusLookup(ReservationStatus value) : base(value)
        {
        }

        public ICollection<Reservation> Reservations { get; set; } = [];
    }

    public class TheaterTypeLookup : EnumLookupBase<TheaterType>
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
