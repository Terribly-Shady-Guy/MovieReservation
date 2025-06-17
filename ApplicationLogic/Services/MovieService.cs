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
            var movies = _dbContext.Movies.AsQueryable<Movie>();

            if (genre is not null)
            {
                movies = movies.Include(m => m.Genres.Where(g => g.Name.StartsWith(genre)));
            }

            return await movies.Select(m => new MovieVM
            {
                MovieId = m.MovieId,
                Description = m.Description,
                Title = m.Title,
                Genres = m.Genres.ToList(),
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
                Description = movie.Description,
                PosterImageName = newFileName
            };

            newMovie.Genres.Add(new Genre { Name = movie.Genre });
            
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
