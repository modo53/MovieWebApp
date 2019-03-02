using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieWebApp.Data;
using MovieWebApp.Models;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MovieWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UpdateMoviesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/UpdateMovies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
        {
            TMDbClient tClient = new TMDbClient(_configuration["TMDb:APIKey"]);
            try
            {
                await StoreUpcommingData(tClient);
                await StorePopularData(tClient);
                await StoreTopRatedData(tClient);
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
            return await _context.Movie.ToListAsync();
        }
        private async Task StorePopularData(TMDbClient client)
        {
            if (client == null)
            {
                return;
            }
            SearchContainer<SearchMovie> results = client.GetMoviePopularListAsync("ja-jp").Result;
            await DbTableUpdate(searchMovieResult: results, category: "Popular");

        }
        private async Task StoreTopRatedData(TMDbClient client)
        {
            if (client == null)
            {
                return;
            }
            SearchContainer<SearchMovie> results = client.GetMovieTopRatedListAsync("ja-jp").Result;
            await DbTableUpdate(searchMovieResult: results, category: "TopRated");

        }
        private async Task StoreUpcommingData(TMDbClient client)
        {
            if (client == null)
            {
                return;
            }
            SearchContainerWithDates<SearchMovie> results = client.GetMovieUpcomingListAsync("ja-jp").Result;
            await DbTableUpdate(searchMovieResult: results, category: "Upcoming");

        }
        private async Task DbTableUpdate(SearchContainer<SearchMovie> searchMovieResult, string category = "no category")
        {
            if (searchMovieResult == null)
            {
                return;
            }
            foreach (SearchMovie tmdbMovie in searchMovieResult.Results)
            {
                Movie apiUpdateMovies = await _context.Movie.FirstOrDefaultAsync(m => m.Movieid == tmdbMovie.Id);
                if (apiUpdateMovies == null)
                {
                    Movie apiAddMovies = new Movie();
                    apiAddMovies.Movieid = tmdbMovie.Id;
                    apiAddMovies.Title = tmdbMovie.Title;
                    apiAddMovies.ReleaseDate = (DateTime)tmdbMovie.ReleaseDate;
                    apiAddMovies.popularity = (decimal)tmdbMovie.Popularity;
                    apiAddMovies.vote_average = (decimal)tmdbMovie.VoteAverage;
                    apiAddMovies.Category = category;
                    _context.Add(apiAddMovies);
                }
                else
                {
                    apiUpdateMovies.Movieid = tmdbMovie.Id;
                    apiUpdateMovies.Title = tmdbMovie.Title;
                    apiUpdateMovies.ReleaseDate = (DateTime)tmdbMovie.ReleaseDate;
                    apiUpdateMovies.popularity = (decimal)tmdbMovie.Popularity;
                    apiUpdateMovies.vote_average = (decimal)tmdbMovie.VoteAverage;
                    apiUpdateMovies.Category = category;
                    _context.Update(apiUpdateMovies);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
