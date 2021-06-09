using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Application.QueueWorkItems;
using ProductReviewApp.Infrastructure.Queue;
using ProductReviewApp.Persistence.Enums;
using ProductReviewApp.Persistence.Models;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.CommandHandlers
{
    public class CreateProductReviewCommandHandler: IRequestHandler<CreateProductReviewCommand, int>
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IProductRepository _productRepository;
        private readonly ActualizeProductReviewQueueWorkItem _actualizeProductReviewQueueWorkItem;

        public CreateProductReviewCommandHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            IProductRepository productRepository,
            ActualizeProductReviewQueueWorkItem actualizeProductReviewQueueWorkItem)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _productRepository = productRepository;
            _actualizeProductReviewQueueWorkItem = actualizeProductReviewQueueWorkItem;
        }

        public Task<int> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Asin))
                throw new ArgumentNullException(nameof(request.Asin));

            var product = _productRepository.GetByAsin(request.Asin);
            if (product != null)
                throw new InvalidOperationException($"Product with '{request.Asin}' ASIN already exists");

            var id =_productRepository.Add(new Product
            {
                Asin = request.Asin,
                Status = ProductStatus.NotProcessed
            });

            _backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
            {
                await _actualizeProductReviewQueueWorkItem.DoWork(request.Asin);
            });

            return Task.FromResult(id);
        }
    }
}
