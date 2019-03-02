﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebApp.Data;
using MovieWebApp.Models;
using Microsoft.Extensions.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MovieWebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MoviesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string category)
        {
            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(category))
            {
                movies = movies.Where(c => c.Category.Contains(category));
                ViewData["Category"] = category;
            }

            TMDbClient client = new TMDbClient(_configuration["TMDb:APIKey"]);
            SearchContainerWithDates<SearchMovie> results = client.GetMovieUpcomingListAsync("ja-jp").Result;
            /*
            foreach (SearchMovie tmdbMovie in results.Results)
            {
                var getMovie = await _context.Movie.FirstOrDefaultAsync(m => m.ID == tmdbMovie.Id);
                if (getMovie == null)
                {

                    getMovie.Movieid = tmdbMovie.Id;
                    getMovie.Title = tmdbMovie.Title;
                    getMovie.ReleaseDate = (DateTime) tmdbMovie.ReleaseDate;
                    getMovie.popularity = (decimal)tmdbMovie.Popularity;
                    getMovie.vote_average = (decimal)tmdbMovie.VoteAverage;
                    getMovie.Category = "Upcoming";
                    
                    _context.Add(getMovie);
                }
                else
                {
                    getMovie.Movieid = tmdbMovie.Id;
                    getMovie.Title = tmdbMovie.Title;
                    getMovie.ReleaseDate = (DateTime)tmdbMovie.ReleaseDate;
                    getMovie.popularity = (decimal)tmdbMovie.Popularity;
                    getMovie.vote_average = (decimal)tmdbMovie.VoteAverage;
                    getMovie.Category = "Upcoming";
                    _context.Update(getMovie);
                }
                await _context.SaveChangesAsync();
            }
            */
            ViewData["TMDbTitle"] = results.Results[0].Title;
            ViewData["Results"] = results.Results;
            
            return View(await movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Movieid,Title,ReleaseDate,popularity,vote_average,Category")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Movieid,Title,ReleaseDate,popularity,vote_average,Category")] Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }
    }
}
