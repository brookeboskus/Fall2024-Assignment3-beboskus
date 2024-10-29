using Fall2024_Assignment3_beboskus.Models;

namespace Fall2024_Assignment3_beboskus.ViewModels
{
    public class MovieReviewsViewModel
    {
        public Movie Movie { get; set; }
        public string MovieTitle => Movie?.Title;
        public int MovieId => Movie?.Id ?? 0;
        public List<ReviewSentiment> Reviews { get; set; }
    }

    public class ReviewSentiment
    {
        public string Review { get; set; }
        public string Sentiment { get; set; }
    }
}
