using Microsoft.Extensions.DependencyInjection;
using ProductReviewApp.Infrastructure.Queue;

namespace ProductReviewApp.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueueService(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
