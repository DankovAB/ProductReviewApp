using MediatR;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Application.Queries
{
    public class GetProductByAsinQuery: IRequest<Product>
    {
        public string Asin { get; }
        public GetProductByAsinQuery(string asin)
        {
            Asin = asin;
        }
    }
}
