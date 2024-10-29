using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fall2024_Assignment3_beboskus.Models
{
    public class Movie
    {
        public int Id { get; set; }


        public string Title { get; set; }


        public string Genre { get; set; }


        public int Year { get; set; }


        public string IMDBLink { get; set; }

        public string PosterUrl { get; set; }

        // Navigation property
        public ICollection<Actor> Actors { get; set; }


        public Movie()
        {
            Actors = new List<Actor>();
        }
    }
}

