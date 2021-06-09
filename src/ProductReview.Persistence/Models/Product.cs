using System;
using System.Collections.Generic;
using ProductReviewApp.Persistence.Enums;

namespace ProductReviewApp.Persistence.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Asin { get; set; }
        public string Link { get; set; }
        public List<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public int ReviewCount { get; set; }
        public DateTime? OnLastReview { get; set; }
        public DateTime? OnActualized { get; set; }
        public ProductStatus Status { get; set; }
    }
}
