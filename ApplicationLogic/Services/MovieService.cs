using ApplicationLogic.ViewModels;
using DbInfrastructure;
using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;

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
            var result = await _dbContext.Movies
                .AsNoTracking()
                .SelectMany(m => m.Genres.Where(g => g.Name.StartsWith(genre ?? "")), (m, g) => new
                {
                    m.MovieId,
                    m.Description,
                    m.PosterImageName,
                    m.Title,
                    GenreName = g.Name,
                })
                .GroupBy(m => new
                {
                    m.MovieId,
                    m.PosterImageName,
                    m.Description,
                    m.Title,
                })
                .ToListAsync();

            return result.Select(x => new MovieVM
            {
                Description = x.Key.Description,
                MovieId = x.Key.MovieId,
                Title = x.Key.Title,
                PosterImageName = x.Key.PosterImageName,
                Genres = x.Select(g => g.GenreName).ToList(),
            }).ToList();
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

            Dictionary<string, Genre> genres = await _dbContext.Genres
                .Where(g => movie.Genres.Contains(g.Name))
                .ToDictionaryAsync(g => g.Name);

            foreach (string genreName in movie.Genres)
            {
                if (genres.TryGetValue(genreName, out Genre? genre))
                {
                    newMovie.Genres.Add(genre);
                }
                else
                {
                    newMovie.Genres.Add(new Genre { Name = genreName });
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
