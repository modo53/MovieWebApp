using System;
using System.Collections.Generic;
using MovieWebApp.Models;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MovieWebApp.Library
{
    public class TmdbClientLib
    {
        private TMDbClient _tClient;

        public TmdbClientLib(TMDbClient tClient)
        {
            if (null == _tClient)
            {
                _tClient = tClient;
            }
        }

        public virtual List<Movie> getMovies(string category = "Popular")
        {
            List<Movie> movies = new List<Movie>();
            SearchContainer<SearchMovie> results = null;

            switch (category)
            {
                case "TopRated":
                    results = _tClient.GetMovieTopRatedListAsync("ja-jp").Result;
                    break;
                case "Upcoming":
                    results = _tClient.GetMovieUpcomingListAsync("ja-jp").Result;
                    break;

                case "Popular":
                    results = _tClient.GetMoviePopularListAsync("ja-jp").Result;
                    break;
                default:
                    category = "Popular";
                    results = _tClient.GetMoviePopularListAsync("ja-jp").Result;
                    break;
            }
            return SearchContainerToMovies(results, category);
        }
        private List<Movie> SearchContainerToMovies(SearchContainer<SearchMovie> results, string category)
        {
            List<Movie> movies = new List<Movie>();
            foreach (SearchMovie tmdbMovie in results.Results)
            {
                movies.Add(SearchMovieToMovie(tmdbMovie, category));
            }
            return movies;
        }
        private Movie SearchMovieToMovie(SearchMovie tmdbMovie, string category)
        {

            Movie movie = new Movie();
            movie.Movieid = tmdbMovie.Id;
            movie.Title = tmdbMovie.Title;
            movie.ReleaseDate = (DateTime)tmdbMovie.ReleaseDate;
            movie.popularity = (decimal)tmdbMovie.Popularity;
            movie.vote_average = (decimal)tmdbMovie.VoteAverage;
            movie.Category = category;
            return movie;
        }

    }
}
