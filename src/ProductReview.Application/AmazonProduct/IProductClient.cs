using System;
using System.Threading.Tasks;
using ProductReviewApp.Application.AmazonProduct.Models;

namespace ProductReviewApp.Application.AmazonProduct
{
    public interface IProductClient
    {
        Task<ProductSnapshot> GetProductWithReviews(string asin, DateTime? lastReviewsDate = null);
    }
}