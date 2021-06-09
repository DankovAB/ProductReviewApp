using System.Threading.Tasks;
using FluentScheduler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Application.Queries;

namespace ProductReviewApp.Api.Scheduler
{
    public class ActualizeProductReviewScheduler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ActualizeProductReviewSchedulerSettings _settings;

        public ActualizeProductReviewScheduler(
            IOptions<ActualizeProductReviewSchedulerSettings> settings, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
        }

        public void StartActualizeScheduler()
        {
            JobManager.AddJob(async () =>
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new ActualizeProductReviewCommand());
                    }
                },
                s => s.ToRunNow()
                    //s.ToRunEvery(_settings.RunEveryDays).Days().At(_settings.AtHours, _settings.AtMinutes)
            );
        }
    }
}
