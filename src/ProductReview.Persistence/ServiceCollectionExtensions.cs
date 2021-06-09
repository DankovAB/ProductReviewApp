using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiteDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureDatabaseSettings(configuration);

            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        private static IServiceCollection ConfigureDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<LiteDbConfig>(configuration.GetSection("LiteDbConfig"));

            return services;
        }
    }
}
