using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Queries;
using ProductReviewApp.Persistence.Models;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.QueryHandlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IList<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IList<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_productRepository.GetAll());
        }
    }
}
