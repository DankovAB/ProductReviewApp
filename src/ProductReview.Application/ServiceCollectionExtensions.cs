using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewApp.Application.AmazonProduct;
using ProductReviewApp.Application.QueueWorkItems;
using ProductReviewApp.Application.Services;
using ProductReviewApp.Application.Settings;
using ProductReviewApp.Application.UserNotification;

namespace ProductReviewApp.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddProductClient(services, configuration);
            AddUserNotification(services, configuration);

            services.AddScoped<ProductReviewService, ProductReviewService>();

            services.AddTransient<ActualizeProductReviewQueueWorkItem, ActualizeProductReviewQueueWorkItem>();
            services.AddOptions()
                .Configure<ActualizeSettings>(configuration.GetSection("ActualizeSettings"));

            return services;
        }

        private static IServiceCollection AddUserNotification(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<EmailActualizeNotificationSettings>(configuration.GetSection("EmailActualizeNotificationSettings"))
                .Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IUserNotification, EmailNotification>();

            return services;
        }

        private static IServiceCollection AddProductClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProductClient, AmazonProductClient>();
            services.AddHttpClient<IProductClient, AmazonProductClient>(c =>
            {
                var settings = configuration.GetSection("AmazonProductSettings").Get<AmazonProductSettings>();
                c.BaseAddress = new Uri(settings.ProductReviewUrl);
                //c.DefaultRequestHeaders.UserAgent.ParseAdd(
                //    "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                //c.DefaultRequestHeaders.Accept.ParseAdd(
                //    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                c.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, sdch");
                c.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.8");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            return services;
        }
    }
}
