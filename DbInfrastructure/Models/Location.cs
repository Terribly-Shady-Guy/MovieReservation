namespace DbInfrastructure.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Zip { get; set; }

        public ICollection<Auditorium> Auditoriums { get; set; } = new List<Auditorium>();
    }
}
