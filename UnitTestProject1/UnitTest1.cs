using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieWebApp.Controllers;
using MovieWebApp.Data;
using MovieWebApp.Library;
using MovieWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private ApplicationDbContext _dbContext;
        private MockClientLib _mockClientLib;

        [TestInitialize]
        public void TestInitialize()
        {
            // set test data in model on Memory Databases. use UpdateMoviesController.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("MovieAppUnitTest");
            _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            _mockClientLib = new MockClientLib();
        }

        private bool CompareMoviesList(List<Movie> movies1, List<Movie> movies2)
        {
            return movies1.SequenceEqual(movies2, new CompareMovie());

        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        [TestMethod]
        public void TestCompareMoviesList()
        {
            var expectedTopRatedMovieList1 = _mockClientLib.getMovies("TopRated");
            var expectedTopRatedMovieList2 = _mockClientLib.getMovies("TopRated");
            Assert.IsTrue(CompareMoviesList(expectedTopRatedMovieList1, expectedTopRatedMovieList2), "CompareMoviesList");
        }

        [DataTestMethod()]
        [DataRow("Popular", "Popular")]
        [DataRow("TopRated", "TopRated")]
        [DataRow("Upcoming", "Upcoming")]
        [DataRow("Pop", "Pop")]
        public async Task TestGetTMDbMovieFromApi(string category, string mockCategory)
        {
            List<Movie> movies = new List<Movie>();
            UpdateMoviesController updateCont = new UpdateMoviesController(_dbContext, null, _mockClientLib);

            movies = updateCont.GetTMDbMovieFromApi(movies, category);

            Assert.IsTrue(CompareMoviesList(movies, _mockClientLib.getMovies(mockCategory)), "CompareMoviesList "+ category);
        }


        [DataTestMethod()]
        [DataRow("Popular")]
        [DataRow("TopRated")]
        [DataRow("Upcoming")]
        public async Task TestMovieTableUpdate(string category)
        {

            UpdateMoviesController updateCont = new UpdateMoviesController(_dbContext, null, _mockClientLib);
            MoviesController moviesCont = new MoviesController(_dbContext);

            List<Movie> insertMovieList = new List<Movie> { DammyMovieData(category) };
            await updateCont.MovieTableUpdate(insertMovieList);

            var movies = from m in _dbContext.Movie select m;
            movies = movies.Where(c => c.Category.Contains(category));
            List<Movie> dbList = await movies.ToListAsync();
            Assert.IsTrue(CompareMoviesList(insertMovieList, dbList), category + " data Update check");
        }

        [DataTestMethod()]
        [DataRow("Popular")]
        [DataRow("TopRated")]
        [DataRow("Upcoming")]
        public async Task TestMoviesControllerView(string category)
        {

            UpdateMoviesController updateCont = new UpdateMoviesController(_dbContext, null, _mockClientLib);
            MoviesController moviesCont = new MoviesController(_dbContext);

            // Initial Insert.
            List<Movie> insertMovieList = new List<Movie> {
                DammyMovieData("Popular"),
                DammyMovieData("TopRated"),
                DammyMovieData("Upcoming")
            };
            await updateCont.MovieTableUpdate(insertMovieList);
            var resultPopular = await moviesCont.Index(category) as ViewResult;

            Assert.IsInstanceOfType(resultPopular, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)resultPopular;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<Movie>));
            List<Movie> model = viewResult.ViewData.Model as List<Movie>;

            List<Movie> compareMovieList = new List<Movie> {DammyMovieData(category)};
            Assert.IsTrue(CompareMoviesList(model, compareMovieList), category + " data check");

            // Update.
            List<Movie> movies = new List<Movie>();
            movies = updateCont.GetTMDbMovieFromApi(movies, "Popular");
            movies = updateCont.GetTMDbMovieFromApi(movies, "TopRated");
            movies = updateCont.GetTMDbMovieFromApi(movies, "Upcoming");
            await updateCont.MovieTableUpdate(movies);

            // after Update.
            resultPopular = await moviesCont.Index(category) as ViewResult;
            
            Assert.IsInstanceOfType(resultPopular, typeof(ViewResult));
            viewResult = (ViewResult)resultPopular;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<Movie>));
            model = viewResult.ViewData.Model as List<Movie>;
            var expectedMovieList = _mockClientLib.getMovies(category);
                
            Assert.IsTrue(CompareMoviesList(model, expectedMovieList), category + " data check");

        }
        private Movie DammyMovieData(string category = "Popular")
        {
            Movie tempMovie = new Movie();

            switch (category)
            {
                case "TopRated":
                    tempMovie.ID = 2;
                    tempMovie.vote_average = 9;
                    tempMovie.Movieid = 456;
                    tempMovie.popularity = 6;
                    tempMovie.ReleaseDate = DateTime.Parse("2018/02/01");
                    tempMovie.Title = "old title TopRated";
                    tempMovie.Category = "TopRated";
                    break;

                case "Upcoming":
                    tempMovie.ID = 3;
                    tempMovie.vote_average = 5;
                    tempMovie.Movieid = 789;
                    tempMovie.popularity = 2;
                    tempMovie.ReleaseDate = DateTime.Parse("2018/03/01");
                    tempMovie.Title = "old title Upcoming";
                    tempMovie.Category = "Upcoming";
                    break;

                case "Popular":
                default:
                    tempMovie.ID = 1;
                    tempMovie.vote_average = 2;
                    tempMovie.Movieid = 123;
                    tempMovie.popularity = 11;
                    tempMovie.ReleaseDate = DateTime.Parse("2018/01/01");
                    tempMovie.Title = "old title Popular";
                    tempMovie.Category = "Popular";
                    break;
            }

            return tempMovie;
        }

    }

    public class MockClientLib : TmdbClientLib
    {
        public MockClientLib(TMDbClient tClient = null)
            :base(tClient)
        {

        }
        public override List<Movie> getMovies(string category = "Popular")
        {
            Movie tempMovie = new Movie();
            List<Movie> movieList = new List<Movie>();

            switch (category)
            {
                case "TopRated":
                    tempMovie.ID = 2;
                    tempMovie.vote_average = 9;
                    tempMovie.Movieid = 456;
                    tempMovie.popularity = 6;
                    tempMovie.ReleaseDate = DateTime.Parse("2019/02/01");
                    tempMovie.Title = "title TopRated";
                    tempMovie.Category = "TopRated";
                    break;

                case "Upcoming":
                    tempMovie.ID = 3;
                    tempMovie.vote_average = 5;
                    tempMovie.Movieid = 789;
                    tempMovie.popularity = 2;
                    tempMovie.ReleaseDate = DateTime.Parse("2019/03/01");
                    tempMovie.Title = "title Upcoming";
                    tempMovie.Category = "Upcoming";
                    break;

                case "Popular":
                default:
                    tempMovie.ID = 1;
                    tempMovie.vote_average = 2;
                    tempMovie.Movieid = 123;
                    tempMovie.popularity = 11;
                    tempMovie.ReleaseDate = DateTime.Parse("2019/01/01");
                    tempMovie.Title = "title Popular";
                    tempMovie.Category = "Popular";
                    break;
            }
            movieList.Add(tempMovie);
            return movieList;
        }
    }
    public class CompareMovie : IEqualityComparer<Movie>
    {
        public bool Equals(Movie x, Movie y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            return x != null && y != null 
                && x.ID.Equals(y.ID) 
                && x.Movieid.Equals(y.Movieid)
                && x.Title.Equals(y.Title)
                && x.ReleaseDate.Equals(y.ReleaseDate)
                && x.popularity.Equals(y.popularity)
                && x.vote_average.Equals(y.vote_average)
                && x.Category.Equals(y.Category);
        }

        public int GetHashCode(Movie obj)
        {
            int hashId = obj.ID.GetHashCode();
            int hashMovieid = obj.Movieid.GetHashCode();

            return hashId ^ hashMovieid;
        }
    }
}
