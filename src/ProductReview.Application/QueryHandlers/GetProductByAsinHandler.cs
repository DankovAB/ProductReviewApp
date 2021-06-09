using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Queries;
using ProductReviewApp.Persistence.Models;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.QueryHandlers
{
    public class GetProductByAsinQueryHandler : IRequestHandler<GetProductByAsinQuery, Product>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByAsinQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product> Handle(GetProductByAsinQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_productRepository.GetByAsin(request.Asin));
        }
    }
}
