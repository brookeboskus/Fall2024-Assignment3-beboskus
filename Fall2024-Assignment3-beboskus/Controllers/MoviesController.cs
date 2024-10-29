using System;
using Fall2024_Assignment3_beboskus.Models;
using Fall2024_Assignment3_beboskus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Fall2024_Assignment3_beboskus.ViewModels;
using Azure.AI.OpenAI;
using VaderSharp2;
using System.Linq;
using System.Collections.Generic;
using Azure;
using Mono.TextTemplating;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fall2024_Assignment3_beboskus.Controllers
{
    public class MoviesController : Controller
	{
        private readonly ApplicationDbContext _context;
        //private readonly OpenAIClient _openAIClient;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
            //_openAIClient = openAIClient;
        }

        //GET: Movies/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Genre, Year, IMDBLink, PosterUrl")] Movie movie)
        {
            Console.WriteLine("Create action was hit");

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("valid model state");
                return RedirectToAction(nameof(Index));

            }
            Console.WriteLine("invalid model");
            return View(movie);
        }

        //GET: Movies/Delete/
        [HttpGet]
        public async Task<IActionResult> Delete(int? id){
            if (id == null)
            {
                return NotFound();

            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the movie by ID and include the related actors
            var movie = await _context.Movie
                .Include(m => m.Actors)  // Fetch the actors associated with the movie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Create a ViewModel if necessary, or just pass the movie directly
            return View(movie);
        }

        //// GET: Movies/Reviews/{id}
        //[HttpGet]
        //public async Task<IActionResult> Reviews(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    // Find the movie by its ID
        //    var movie = await _context.Movie
        //        .Include(m => m.Actors)  // Include actors if needed
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    //var aiReviews = await GenerateAIReviews(movie.Title);

        //    //var reviewSentiments = aiReviews.Select(r => new ReviewSentiment
        //    //{
        //    //    Review = r,
        //    //    Sentiment = "Neutral"
        //    //}).ToList();

        //    //// Here we'll use placeholder reviews for now
        //    //var reviews = new List<ReviewSentiment>
        //    //{
        //    //    new ReviewSentiment { Review = "Amazing movie with great acting!", Sentiment = "Positive" },
        //    //    new ReviewSentiment { Review = "The plot was confusing and hard to follow.", Sentiment = "Negative" },
        //    //    new ReviewSentiment { Review = "It had some good moments, but overall just okay.", Sentiment = "Neutral" }
        //    //};

        //    // Create a ViewModel to pass to the view
        //    var viewModel = new MovieReviewsViewModel
        //     {
        //        Movie = movie,
        //        Reviews = reviewSentiments
        //     };

        //     return View(viewModel);
        //}

 
        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            _context.Movie.Remove(movie);  // Remove the movie from the database
            await _context.SaveChangesAsync();  // Save changes to the database
            return RedirectToAction(nameof(Index));  // Redirect to the movie list
        }

        //GET: Movies/Edit/{id}
        [HttpGet]
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

        //POST: Movies/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Genre, Year, IMDBLink, PosterUrl")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);  // Update the movie in the database
                    await _context.SaveChangesAsync();  // Save changes
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect back to the movie list after edit
            }
            return View(movie);
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }

        //GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        //// Temporary movie list for demonstration purposes
        //private static List<Movie> Movies = new List<Movie>
        //{
        //   new Movie { Id = 1, Title = "The Matrix", IMDBLink = "https://www.imdb.com/title/tt0133093/", Genre = "Action", Year = 1999, PosterUrl = "/images/matrix.jpg" },
        //   new Movie { Id = 2, Title = "Inception", IMDBLink = "https://www.imdb.com/title/tt1375666/", Genre = "Sci-Fi", Year = 2010, PosterUrl = "/images/inception.jpg" },
        //   new Movie { Id = 3, Title = "Titanic", IMDBLink = "https://www.imdb.com/title/tt0120338/", Genre = "Romance", Year = 1997, PosterUrl = "/images/titanic.jpg" }
        //};

        //// Action method to display the list of movies
        //public IActionResult Index()
        //{
        //   return View(Movies);
        //}
    }
}

