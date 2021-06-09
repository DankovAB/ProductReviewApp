using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductReviewApp.Application.AmazonProduct;
using ProductReviewApp.Application.AmazonProduct.Models;
using ProductReviewApp.Application.Notifications;
using ProductReviewApp.Persistence.Enums;
using ProductReviewApp.Persistence.Models;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.Services
{
    public class ProductReviewService
    {
        private readonly IProductClient _productClient;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductReviewService> _logger;
        private readonly IMediator _mediator;

        public ProductReviewService(
            IProductClient productClient,
            IProductRepository productRepository,
            ILogger<ProductReviewService> logger,
            IMediator mediator)
        {
            _productClient = productClient;
            _productRepository = productRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task StartProcessing(string asin, DateTime? lastReviewsDate = null)
        {
            if (string.IsNullOrWhiteSpace(asin))
                throw new ArgumentNullException(nameof(asin));

            var product  = _productRepository.GetByAsin(asin);
            if (product is null)
            {
                _logger.LogWarning($"Product with '{asin}' ASIN not exists");
                return;
            }

            if (product.Status != ProductStatus.NotProcessed
                    && product.Status != ProductStatus.Succeeded
                    && product.Status != ProductStatus.Failed)
                throw new InvalidOperationException(
                    $"Product with '{asin}' ASIN has incorrect '{product.Status}' status");

            product.Status = ProductStatus.Processing;
            _productRepository.Update(product);

            try
            {
                var productSnapshot = await _productClient.GetProductWithReviews(asin, lastReviewsDate);
                UpdateProductModel(product, productSnapshot);

                product.Status = ProductStatus.Succeeded;
                product.OnActualized = DateTime.UtcNow;
                _productRepository.Update(product);
            }
            catch(Exception ex)
            {
                product.Status = ProductStatus.Failed;
                _productRepository.Update(product);

                _logger.LogError(ex, "StartProcessing is failed");
            }

            //TODO it would be better use queue instead of mediator or no waiting publish event at least
            //also we need to publish unsuccessful notification
            if (product.Status == ProductStatus.Succeeded)
                await _mediator.Publish(new ProductReviewActualizedNotification(product.Asin), CancellationToken.None);
        }

        private void UpdateProductModel(Product product, ProductSnapshot snapshotProductSnapshot)
        {
            product.Link = snapshotProductSnapshot.Link;
            var snapshotReviews = snapshotProductSnapshot
                .ProductReviews
                .Select(r => new ProductReview
            {
                Date = r.Date,
                ProfileName = r.ProfileName,
                Rating = r.Rating,
                Review = r.Review,
                Title = r.Title
            });

            product.ProductReviews.AddRange(snapshotReviews);
            product.ReviewCount = product.ProductReviews.Count;
            product.OnLastReview = product.ProductReviews.DefaultIfEmpty().Max(r => r.Date);
        }
    }
}
