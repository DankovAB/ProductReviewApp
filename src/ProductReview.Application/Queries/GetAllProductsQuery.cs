using System.Collections.Generic;
using MediatR;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Application.Queries
{
    public class GetAllProductsQuery: IRequest<IList<Product>>
    {
    }
}
