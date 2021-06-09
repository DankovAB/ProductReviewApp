using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Options;
using ProductReviewApp.Persistence.Enums;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly LiteDbConfig _configs;

        public ProductRepository(IOptions<LiteDbConfig> configs)
        {
            _configs = configs.Value;
        }

        public int Add(Product product)
        {
            using (var productDb = new ProductCollection(_configs))
            {
               return productDb.Collection.Insert(product).AsInt32;
            }
        }

        public void Update(Product product)
        {
            using (var productDb = new ProductCollection(_configs))
            {
                productDb.Collection.Update(product);
            }
        }

        public void Delete(Product product)
        {
            Delete(product.Id);
        }

        public void Delete(int id)
        {
            using (var productDb = new ProductCollection(_configs))
            {
                productDb.Collection.Delete(id);
            }
        }

        public Product GetByAsin(string asin)
        {
            using (var productDb = new ProductCollection(_configs))
            {
                return productDb.Collection
                    .Query()
                    .Where(r => r.Asin.Equals(asin, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

            }
        }

        public IList<ProductReview> GetReviewsByAsin(string asin, DateTime? fromReviewDate = null)
        {
            using (var productDb = new ProductCollection(_configs))
            {
                var product = productDb.Collection
                    .Query()
                    .Where(r => r.Asin.Equals(asin, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (fromReviewDate.HasValue)
                    return product.ProductReviews
                        .Where(r => r.Date >= fromReviewDate.Value)
                        .ToList();

                return product.ProductReviews;
            }
        }

        public IList<Product> GetAll()
        {
            using (var productDb = new ProductCollection(_configs))
            {
                return productDb.Collection.FindAll().ToList();
            }
        }

        public List<Product> GetOutOfDate(DateTime beforeActualizeDate)
        {
            using (var productDb = new ProductCollection(_configs))
            {
                return productDb.Collection.Query()
                    .Where(r => (r.OnActualized <= beforeActualizeDate
                                            || r.OnActualized == null)
                                && (r.Status == ProductStatus.Succeeded
                                        || r.Status == ProductStatus.Failed))
                    .ToList();
            }
        }

        private class ProductCollection: IDisposable
        {
            private const string CollectionName = "Product";
            private readonly LiteDatabase _liteDatabase;
            public ILiteCollection<Product> Collection { get; }

            public ProductCollection(LiteDbConfig configs)
            {
                _liteDatabase = new LiteDatabase(configs.DatabasePath);
                Collection = _liteDatabase.GetCollection<Product>(CollectionName);
            }

            public void Dispose()
            {
                _liteDatabase?.Dispose();
            }
        }
    }
}
