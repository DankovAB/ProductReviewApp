using MediatR;

namespace ProductReviewApp.Application.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public string Asin { get; }

        public DeleteProductCommand(string asin)
        {
            Asin = asin;
        }
    }
}
