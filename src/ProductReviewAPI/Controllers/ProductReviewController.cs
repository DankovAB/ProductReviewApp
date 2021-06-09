using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProductReviewApp.Api.Models;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Application.Queries;
using ProductReviewApp.Application.UserNotification;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{asin}")]
        public async Task<IList<ProductReview>> Get(string asin, [FromQuery] DateTime? fromDate)
        {
            if (string.IsNullOrWhiteSpace(asin))
                throw new BadHttpRequestException("ASIN couldn't be empty");

            return await _mediator.Send(new GetProductReviewByAsinAndDateQuery(asin, fromDate));
        }
    }
}
