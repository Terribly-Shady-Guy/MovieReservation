using Microsoft.EntityFrameworkCore;
using MovieReservation.Models;
using MovieReservation.ViewModels;

namespace MovieReservation.Services
{
    public class MovieService
    {
        private readonly MovieReservationDbContext _dbContext;

        public MovieService(MovieReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MovieVM>> GetMovies(MovieSearch searchParams)
        {
            var movies = _dbContext.Movies.AsQueryable<Movie>();

            if (searchParams.Genre is not null)
            {
                movies = movies.Where(m => m.Genre.StartsWith(searchParams.Genre));
            }

            if (searchParams.Title is not null)
            {
                movies = movies.Where(m => m.Title.StartsWith(searchParams.Title));
            }

            return await movies.Select(m => new MovieVM
            {
                MovieId = m.MovieId,
                Description = m.Description,
                Title = m.Title,
                Genre = m.Genre,
                PosterImageName = m.PosterImageName
            })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddMovie(MovieUploadVM movie)
        {
            var newMovie = new Movie
            {
                Title = movie.Title,
                Genre = movie.Genre,
                Description = movie.Description,
                PosterImageName = movie.PosterImage.FileName
            };

            _dbContext.Movies.Add(newMovie);
            await _dbContext.SaveChangesAsync();
        }
    }
}
