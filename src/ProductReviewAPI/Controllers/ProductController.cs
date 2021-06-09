using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProductReviewApp.Api.Models;
using ProductReviewApp.Application.Commands;
using ProductReviewApp.Application.Queries;
using ProductReviewApp.Persistence.Models;

namespace ProductReviewApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _mediator.Send(new GetAllProductsQuery());
        }

        [HttpGet("{asin}")]
        public async Task<Product> Get(string asin)
        {
            if (string.IsNullOrWhiteSpace(asin))
                throw new BadHttpRequestException("ASIN couldn't be empty");

            return await _mediator.Send(new GetProductByAsinQuery(asin));
        }

        [HttpPost]
        public async Task Post(CreateProductRequest createProductRequest)
        {
            if (string.IsNullOrWhiteSpace(createProductRequest.Asin))
                throw new BadHttpRequestException("ASIN couldn't be empty");

            await _mediator.Send(new CreateProductReviewCommand(createProductRequest.Asin));
        }

        [HttpDelete("{asin}")]
        public async Task Delete(string asin)
        {
            if (string.IsNullOrWhiteSpace(asin))
                throw new BadHttpRequestException("ASIN couldn't be empty");

            await _mediator.Send(new DeleteProductCommand(asin));
        }
    }
}
