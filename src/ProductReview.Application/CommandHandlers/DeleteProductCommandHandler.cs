using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Persistence.Repositories;

namespace ProductReviewApp.Application.CommandHandlers
{
    public class DeleteProductCommandHandler: IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Asin))
                throw new ArgumentNullException(nameof(request.Asin));

            var product = _productRepository.GetByAsin(request.Asin);
            if (product is null)
                throw new InvalidOperationException($"Product with '{request.Asin}' ASIN not found");

            _productRepository.Delete(product);

            return Task.FromResult(new Unit());
        }
    }
}
