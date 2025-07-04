﻿namespace DbInfrastructure.Models
{
    public class InternalLogin
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public DateTime LoginDate { get; set; } = DateTime.UtcNow;

        public required AppUser LoggedInUser { get; set; }
    }
}
