using System.Collections.Generic;

namespace ProductReviewApp.Application.AmazonProduct.Models
{
    public class ProductSnapshot
    {
        public string Asin { get; set; }
        public string Link { get; set; }
        public List<ProductReviewSnapshot> ProductReviews { get; set; } = new List<ProductReviewSnapshot>();
    }
}
