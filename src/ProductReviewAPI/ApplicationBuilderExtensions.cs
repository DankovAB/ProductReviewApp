using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using ProductReviewApp.Api.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ProductReviewApp.Api
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseScheduler(this IApplicationBuilder app)
        {
            JobManager.Initialize();
            var scheduler = app.ApplicationServices.GetRequiredService<ActualizeProductReviewScheduler>();

            scheduler.StartActualizeScheduler();

            return app;
        }
    }
}
