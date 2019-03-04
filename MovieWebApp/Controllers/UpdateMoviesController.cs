using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieWebApp.Data;
using MovieWebApp.Library;
using MovieWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private TmdbClientLib _tmdbClient;

        public UpdateMoviesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public TmdbClientLib GetTMDbClient()
        {
            if (null == _tmdbClient)
            {
                _tmdbClient = new TmdbClientLib(_configuration["TMDbAPIKey"]);
            }
            
            return _tmdbClient;
        }

        // GET: api/UpdateMovies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
        {
            await GetTMDbMovie(GetTMDbClient());
            return await _context.Movie.ToListAsync();
        }

        public async Task GetTMDbMovie(TmdbClientLib tmdbClient)
        {
            try
            {
                List<Movie> movies = new List<Movie>();
                movies.AddRange(tmdbClient.getMovies("Popular"));
                movies.AddRange(tmdbClient.getMovies("TopRated"));
                movies.AddRange(tmdbClient.getMovies("Upcoming"));
                await MovieTableUpdate(movies);
            }
            catch (System.AggregateException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (System.ObjectDisposedException)
            {
                throw;
            }
            return;

        }

        private async Task MovieTableUpdate(List<Movie> movies)
        {
            if (movies == null)
            {
                return;
            }
            foreach (Movie movie in movies)
            {
                Movie apiUpdateMovie = await _context.Movie.FirstOrDefaultAsync(m => m.Movieid == movie.Movieid);
                if (apiUpdateMovie == null)
                {

                    _context.Add(movie);
                }
                else
                {
                    apiUpdateMovie.Movieid = movie.Movieid;
                    apiUpdateMovie.Title = movie.Title;
                    apiUpdateMovie.ReleaseDate = movie.ReleaseDate;
                    apiUpdateMovie.popularity = movie.popularity;
                    apiUpdateMovie.vote_average = movie.vote_average;
                    apiUpdateMovie.Category = movie.Category;
                    _context.Update(apiUpdateMovie);

                }
            }
            await _context.SaveChangesAsync();
        }

    }
}
