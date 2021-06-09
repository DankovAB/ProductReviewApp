using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Application.QueueWorkItems;
using ProductReviewApp.Application.Settings;
using ProductReviewApp.Infrastructure.Queue;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.CommandHandlers
{
    public class ActualizeProductReviewCommandHandler : IRequestHandler<ActualizeProductReviewCommand>
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IProductRepository _productRepository;
        private readonly ActualizeSettings _actualizeSettings;
        private readonly ActualizeProductReviewQueueWorkItem _actualizeProductReviewQueueWorkItem;

        public ActualizeProductReviewCommandHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            IProductRepository productRepository,
            IOptions<ActualizeSettings> actualizeSettings,
            ActualizeProductReviewQueueWorkItem actualizeProductReviewQueueWorkItem)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _productRepository = productRepository;
            _actualizeProductReviewQueueWorkItem = actualizeProductReviewQueueWorkItem;
            _actualizeSettings = actualizeSettings.Value;
        }

        public Task<Unit> Handle(ActualizeProductReviewCommand request, CancellationToken cancellationToken)
        {
            var startFrom = DateTime.UtcNow.AddDays(_actualizeSettings.ActualizeEveryDays * -1);
            var outOfDateProducts = _productRepository.GetOutOfDate(startFrom);

            foreach (var outOfDateProduct in outOfDateProducts)
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
                    await _actualizeProductReviewQueueWorkItem.DoWork(outOfDateProduct.Asin,
                        outOfDateProduct.OnLastReview));
            }

            return Task.FromResult(Unit.Value);
        }
    }
}
