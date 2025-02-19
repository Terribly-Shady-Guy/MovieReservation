using Microsoft.EntityFrameworkCore;
using MovieReservation.Data.DbContexts;
using MovieReservation.Models;
using MovieReservation.ViewModels;

namespace MovieReservation.Services
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
            var movies = _dbContext.Movies.AsQueryable<Movie>();

            if (genre is not null)
            {
                movies = movies.Where(m => m.Genre.StartsWith(genre));
            }

            return await movies.Select(m => new MovieVM
            {
                MovieId = m.MovieId,
                Description = m.Description,
                Title = m.Title,
                Genre = m.Genre,
                PosterImageName = _fileHandler.CreateImagePath(m.PosterImageName)
            })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddMovie(MovieFormDataBody movie)
        {
            string fileExtension = Path.GetExtension(movie.PosterImage.FileName);
            string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), fileExtension);

            var newMovie = new Movie
            {
                Title = movie.Title,
                Genre = movie.Genre,
                Description = movie.Description,
                PosterImageName = newFileName
            };
            
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
