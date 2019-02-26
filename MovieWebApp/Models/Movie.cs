using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieWebApp.Models
{
    public class Movie
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "MovieID")]
        public int Movieid { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Popularity")]
        //[Column(TypeName = "decimal(10, 2)")]
        public decimal popularity { get; set; }

        [Display(Name = "Vote average")]
        //[Column(TypeName = "decimal(10, 2)")]
        public decimal vote_average { get; set; }

        [Display(Name = "Category")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        // Upcoming,Popular,TopRated
        public string Category { get; set; }

    }
}