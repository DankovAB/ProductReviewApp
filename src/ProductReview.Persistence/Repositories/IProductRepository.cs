using System;
using System.Collections.Generic;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Persistence.Repositories
{
    public interface IProductRepository
    {
        int Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        Product GetByAsin(string asin);
        IList<ProductReview> GetReviewsByAsin(string asin, DateTime? fromReviewDate = null);
        IList<Product> GetAll();
        List<Product> GetOutOfDate(DateTime beforeActualizeDate);
    }
}