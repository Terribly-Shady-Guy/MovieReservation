﻿namespace DbInfrastructure.Models
{
    public class ShowingSeat
    {
        public int Id { get; set; }
        public required int ShowingId { get; set; }
        public required int SeatId { get; set; }
        public required decimal Price { get; set; }


        public required Showing Showing { get; set; }
        public required Seat Seat { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
