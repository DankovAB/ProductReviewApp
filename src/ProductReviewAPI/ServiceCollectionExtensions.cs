using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewApp.Api.Scheduler;

namespace ProductReviewApp.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<ActualizeProductReviewSchedulerSettings>(
                    configuration.GetSection("ActualizeProductReviewSchedulerSettings"));

            services.AddSingleton<ActualizeProductReviewScheduler, ActualizeProductReviewScheduler>();

            return services;
        }
    }
}
