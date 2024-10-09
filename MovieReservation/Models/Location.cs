namespace MovieReservation.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public ICollection<Auditorium> Auditoriums { get; set; }
    }
}
