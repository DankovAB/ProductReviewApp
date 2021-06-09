using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Queries;
using ProductReviewApp.Persistence.Models;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.QueryHandlers
{
    public class GetProductReviewByAsinAndDateQueryHandlers : IRequestHandler<GetProductReviewByAsinAndDateQuery, IList<ProductReview>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductReviewByAsinAndDateQueryHandlers(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IList<ProductReview>> Handle(GetProductReviewByAsinAndDateQuery request, CancellationToken cancellationToken)
        {
            return _productRepository
                .GetReviewsByAsin(request.Asin, request.FromDate)
                .OrderByDescending(review => review.Date)
                .ToList();
        }
    }
}
