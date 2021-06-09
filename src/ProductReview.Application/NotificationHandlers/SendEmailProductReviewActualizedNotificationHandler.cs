using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Notifications;
using ProductReviewApp.Application.UserNotification;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.NotificationHandlers
{
    public class SendEmailProductReviewActualizedNotificationHandler:INotificationHandler<ProductReviewActualizedNotification>
    {
        private readonly IUserNotification _userNotification;
        private readonly IProductRepository _productRepository;
        public SendEmailProductReviewActualizedNotificationHandler(IUserNotification userNotification, IProductRepository productRepository)
        {
            _userNotification = userNotification;
            _productRepository = productRepository;
        }

        public async Task Handle(ProductReviewActualizedNotification notification, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(notification.Asin))
                throw new ArgumentNullException(nameof(notification.Asin));

            var product = _productRepository.GetByAsin(notification.Asin);
            if (product == null)
                throw new InvalidOperationException($"Product with '{notification.Asin}' ASIN not exists");

            await _userNotification.NotifyActualizeProduct(product);
        }
    }
}
