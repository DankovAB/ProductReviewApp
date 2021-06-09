using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewApp.Application.Services;

namespace ProductReviewApp.Application.QueueWorkItems
{
    public class ActualizeProductReviewQueueWorkItem
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ActualizeProductReviewQueueWorkItem(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DoWork(string asin, DateTime? lastReviewsDate = null)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var productReviewService = scope.ServiceProvider.GetRequiredService<ProductReviewService>();
                await productReviewService.StartProcessing(asin, lastReviewsDate);
            }
        }
    }
}
