using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieWebApp.Data;
using MovieWebApp.Library;
using MovieWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace MovieWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        private TMDbClient _tmdbClient;
        private TmdbClientLib _tmdbClientLib;
        

        public UpdateMoviesController(
            ApplicationDbContext context, 
            IConfiguration configuration = null,
            TmdbClientLib clientLib =null)
        {
            _context = context;
            _configuration = configuration;
            _tmdbClientLib = clientLib;
        }

        private TMDbClient GetTMDbClient()
        {
            if (null == _tmdbClient)
            {
                _tmdbClient = new TMDbClient(_configuration["TMDbAPIKey"]);

            }
            return _tmdbClient;
        }

        private TmdbClientLib GetTMDbClientLib()
        {
            if (null == _tmdbClientLib)
            {
                _tmdbClientLib = new TmdbClientLib(GetTMDbClient());
            }
            return _tmdbClientLib;
        }

        public List<Movie> GetTMDbMovieFromApi(List<Movie> movies, string category = "Popular")
        {
            try
            {
                movies.AddRange(GetTMDbClientLib().getMovies(category));
            }
            catch (System.AggregateException)
            {
                throw;
            }
            catch (System.ObjectDisposedException)
            {
                throw;
            }
            return movies;
        }

        public async Task MovieTableUpdate(List<Movie> movies)
        {
            if (movies == null)
            {
                return;
            }
            foreach (Movie movie in movies)
            {
                Movie updateMovie = await _context.Movie.FirstOrDefaultAsync(m => m.Movieid == movie.Movieid);
                if (updateMovie == null)
                {
                    // new item insert.
                    _context.Add(movie);
                }
                else
                {
                    // exist item update.
                    updateMovie.Movieid = movie.Movieid;
                    updateMovie.Title = movie.Title;
                    updateMovie.ReleaseDate = movie.ReleaseDate;
                    updateMovie.popularity = movie.popularity;
                    updateMovie.vote_average = movie.vote_average;
                    updateMovie.Category = movie.Category;
                    _context.Update(updateMovie);

                }
            }
            await _context.SaveChangesAsync();
        }

        // GET: api/UpdateMovies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
        {
            await GetTMDbMovie();
            return await _context.Movie.ToListAsync();
        }
        private async Task GetTMDbMovie()
        {
            try
            {
                List<Movie> movies = new List<Movie>();
                movies = GetTMDbMovieFromApi(movies,"Popular");
                movies = GetTMDbMovieFromApi(movies,"TopRated");
                movies = GetTMDbMovieFromApi(movies,"Upcoming");
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
    }
}
