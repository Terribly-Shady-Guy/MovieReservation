namespace DbInfrastructure.Models
{
    public enum TheaterType { Standard, _3D, IMax, DolbyCinema }

    public class Auditorium
    {
        public required string AuditoriumNumber { get; set; }
        
        public required int LocationId { get; set; }
        public TheaterType Type { get; set; }

        public required Location Location { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public required TheaterTypeLookup TypeLookup { get; set; }
    }
}
