using MediatR;

namespace ProductReviewApp.Application.Commands
{
    public class CreateProductReviewCommand: IRequest<int>
    {
        public string Asin { get; }

        public CreateProductReviewCommand(string asin)
        {
            Asin = asin;
        }
    }
}
