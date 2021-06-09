using System;

namespace ProductReviewApp.Persistence.Models
{
    public class ProductReview
    {
        public DateTime Date { get; set; }
        public string ProfileName { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
    }
}
