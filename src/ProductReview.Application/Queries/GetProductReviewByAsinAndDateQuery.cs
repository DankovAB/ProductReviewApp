using System;
using System.Collections.Generic;
using MediatR;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Application.Queries
{
    public class GetProductReviewByAsinAndDateQuery : IRequest<IList<ProductReview>>
    {
        public string Asin { get; }
        public DateTime? FromDate { get; }

        public GetProductReviewByAsinAndDateQuery(string asin, DateTime? fromDate)
        {
            Asin = asin;
            FromDate = fromDate;
        }
    }
}