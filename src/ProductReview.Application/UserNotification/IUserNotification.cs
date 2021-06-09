using System.Threading.Tasks;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Application.UserNotification
{
    public interface IUserNotification
    {
        Task NotifyActualizeProduct(Product product);
    }
}