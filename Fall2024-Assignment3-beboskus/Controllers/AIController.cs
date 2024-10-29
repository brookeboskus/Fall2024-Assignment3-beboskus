using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Fall2024_Assignment3_beboskus.Models;
using Fall2024_Assignment3_beboskus.Data;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_beboskus.ViewModels;
using Newtonsoft.Json;

namespace Fall2024_Assignment3_beboskus.Controllers
{
    public class AIController : Controller
    {

        private static readonly string _apiKey = "FHfyoblAdACdGtLU8mj1j1abE5yDc3kiJr8I6uAIzYG2ElKxLoMeJQQJ99AJACYeBjFXJ3w3AAABACOGHhSj";
        private static readonly string _apiEndpoint = "https://fall2024-beboskus-openai.openai.azure.com/openai/deployments/gpt-35-turbo/chat/completions?api-version=2024-08-01-previews";

        private readonly ApplicationDbContext _context;

        // Inject the ApplicationDbContext into the controller
        public AIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to generate AI reviews using GPT-35 Turbo
        private async Task<List<string>> GenerateAIReviews(string movieTitle)
        {
            var requestReview = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}");
            requestReview.Headers.Add("api-key", $"{_apiKey}");
            var reviews = new List<string>();
            var httpClient = new HttpClient();

            //for (int i = 0; i < 10; i++)  // Generate 10 reviews
            //{
                var requestData = new
                {
                    model = "gpt-35-turbo",  // Model name
                    prompt = $"Write a review for the movie '{movieTitle}'"
                };

                requestReview.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                //// Prepare the HTTP request
                //var requestJson = JsonSerializer.Serialize(requestData);
                //var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                //// Add authorization header
                //httpClient.DefaultRequestHeaders.Clear();

                //httpClient.DefaultRequestHeaders.Add("api-key", $"{_apiKey}");

                //// Make the HTTP POST request to OpenAI API
                //var response = await httpClient.PostAsync(_apiEndpoint, content);
                var response = await httpClient.SendAsync(requestReview);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("API response: " + responseJson);  // Log the response

                    var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseJson);

                    // Extract and add the review
                    var review = responseObject?.Choices.FirstOrDefault()?.Text?.Trim();
                    if (!string.IsNullOrEmpty(review))
                    {
                        Console.WriteLine(review);
                        reviews.Add(review);
                
                    }
                    else
                    {
                        Console.WriteLine("Review is empty for iteration " );
                    }
                }
                else
        {
            Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
            //}
            Console.WriteLine(reviews);
            return reviews;
        }


        // GET: AI/Reviews/{id}
        [HttpGet]
        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the movie from the database (you can adjust this query based on your implementation)
            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Generate AI reviews for the movie
            var aiReviews = await GenerateAIReviews(movie.Title);

            // Map AI-generated reviews to ReviewSentiment model
            var reviewSentiments = aiReviews.Select(r => new ReviewSentiment
            {
                Review = r,
                Sentiment = "Neutral"  // Placeholder for sentiment analysis, update this as needed
            }).ToList();

            // Create a ViewModel to pass the data to the view
            var viewModel = new MovieReviewsViewModel
            {
                Movie = movie,
                Reviews = reviewSentiments
            };

            return View("~/Views/OpenAI/Reviews.cshtml", viewModel);  // Pass the ViewModel to the Reviews view
        }

    }

    // Model for parsing the OpenAI response (you can adjust this based on your actual response structure)
    public class OpenAIResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
