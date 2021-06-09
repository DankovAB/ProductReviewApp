using MediatR;

namespace ProductReviewApp.Application.Notifications
{
    public class ProductReviewActualizedNotification: INotification
    {
        public string Asin { get; }
        public ProductReviewActualizedNotification(string asin)
        {
            Asin = asin;
        }
    }
}
