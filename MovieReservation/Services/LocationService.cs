using MovieReservation.Data.DbContexts;
using MovieReservation.Models;
using MovieReservation.ViewModels;

namespace MovieReservation.Services
{
    public class LocationService
    {
        private readonly MovieReservationDbContext _dbContext;

        public LocationService(MovieReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddLocation(LocationVM location)
        {
            var newLocation = new Location
            {
                City = location.City,
                State = location.State,
                Street = location.Street,
                Zip = location.Zip,
            };

            _dbContext.Add(newLocation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLocation(int id)
        {
            Location? locationToDelete = await _dbContext.Locations.FindAsync(id);

            if (locationToDelete is null)
            {
                return;
            }

            _dbContext.Locations.Remove(locationToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}
