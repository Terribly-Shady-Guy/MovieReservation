using Microsoft.EntityFrameworkCore;
using DbInfrastructure.Models;
using DbInfrastructure;
using ApplicationLogic.ViewModels;

namespace ApplicationLogic.Services
{
    public class MovieService
    {
        private readonly MovieReservationDbContext _dbContext;
        private readonly IFileHandler _fileHandler;

        public MovieService(MovieReservationDbContext dbContext, IFileHandler fileHandler)
        {
            _dbContext = dbContext;
            _fileHandler = fileHandler;
        }

        public async Task<List<MovieVM>> GetMovies(string? genre)
        {
            return await _dbContext.Movies
                .AsNoTracking()
                .Where(m => m.Genres.Any(g => g.Name.StartsWith(genre ?? "")))
                .Select(m => new MovieVM
                {
                    Description = m.Description,
                    MovieId = m.MovieId,
                    Title = m.Title,
                    PosterImageName = _fileHandler.CreateImagePath(m.PosterImageName),
                    Genres = m.Genres.Select(g => g.Name).ToList(),
                })
                .ToListAsync();
        }

        public async Task AddMovie(MovieFormDataBody movie)
        {
            string fileExtension = Path.GetExtension(movie.PosterImage.FileName);
            string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), fileExtension);

            var newMovie = new Movie
            {
                Title = movie.Title,
                Description = movie.Description,
                PosterImageName = newFileName
            };

            Dictionary<string, Genre> genres = _dbContext.Genres
                .Where(g => movie.Genres.Contains(g.Name))
                .ToDictionary(g => g.Name);

            foreach (string genre in movie.Genres)
            {
                if (genres.TryGetValue(genre, out Genre? gen))
                {
                    newMovie.Genres.Add(gen);
                }
                else
                {
                    newMovie.Genres.Add(new Genre { Name = genre });
                }
            }
            
            _dbContext.Movies.Add(newMovie);
            await _dbContext.SaveChangesAsync();

            await _fileHandler.CreateFile(movie.PosterImage, newFileName);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            Movie? movieToDelete = await _dbContext.Movies.FindAsync(id);

            if (movieToDelete is null)
            {
                return false;
            }

            _dbContext.Movies.Remove(movieToDelete);
            await _dbContext.SaveChangesAsync();

            _fileHandler.DeleteFile(movieToDelete.PosterImageName);

            return true;
        }
    }
}
