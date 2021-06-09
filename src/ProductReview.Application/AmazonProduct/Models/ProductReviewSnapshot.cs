using System;

namespace ProductReviewApp.Application.AmazonProduct.Models
{
    public class ProductReviewSnapshot
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string ProfileName { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
    }
}
