using ApplicationLogic.Interfaces;
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

        public async Task<List<MovieListItem>> GetMovies(string? genre)
        {
            var result = await _dbContext.Movies
                .AsNoTracking()
                .Where(m => m.Genres.Any(g => g.Name.StartsWith(genre ?? "")))
                .Select(m => new MovieListItem
                {
                    Id = m.Id,
                    Name = m.Title,
                    Genres = m.Genres
                        .Where(g => g.Name.StartsWith(genre ?? ""))
                        .Select(g => g.Name)
                        .ToList(),
                })
                .ToListAsync();

            return result;
        }

        public async Task<MovieDto?> GetById(int id)
        {
            Movie? movie = await _dbContext.Movies
                .AsNoTracking()
                .Include(m =>  m.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return null;
            }

            return new MovieDto
            {
                Description = movie.Description,
                MovieId = movie.Id,
                ImageFileName = movie.ImageFileName,
                Title = movie.Title,
                Genres = movie.Genres
                    .Select(g => g.Name)
                    .ToList(),
            };
        }

        public async Task<int> AddMovie(MovieFormDataBody movie)
        {
            string uploadedFileExtension = Path.GetExtension(movie.PosterImage.FileName);
            string newUploadedFileName = Path.ChangeExtension(Path.GetRandomFileName(), uploadedFileExtension);

            var newMovie = new Movie
            {
                Title = movie.Title,
                Description = movie.Description,
                ImageFileName = newUploadedFileName
            };

            Dictionary<string, Genre> genres = await _dbContext.Genres
                .Where(g => movie.Genres.Contains(g.Name))
                .ToDictionaryAsync(g => g.Name.ToUpper());

            foreach (string genreName in movie.Genres)
            {
                if (genres.TryGetValue(genreName.ToUpper(), out Genre? genre))
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

            await _fileHandler.CreateFile(movie.PosterImage, newUploadedFileName);

            return newMovie.Id;
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

            _fileHandler.DeleteFile(movieToDelete.ImageFileName);

            return true;
        }
    }
}
