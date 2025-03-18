using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagementService.Domain.Entities;
using ProductManagementService.Features.Products.Commands.Create;
using ProductManagementService.Features.Products.Commands.Delete;
using ProductManagementService.Features.Products.Commands.Update;
using ProductManagementService.Features.Products.Queries.Detail;
using ProductManagementService.Features.Products.Queries.List;

namespace ProductManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll([FromQuery] int? organizationId)
        {
            var query = new GetAllProductsQuery
            {
                Filter = new ProductFilter
                {
                    
                    OrganizationId = organizationId
                }
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetById(int productId, [FromQuery] int? organizationId)
        {
            var query = new GetProductByIdQuery { Id = productId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            var command = new CreateProductCommand
            {
                Product = product
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { code = result }, result);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, Product product)
        {
            var command = new UpdateProductCommand
            {
                ProductId = productId,
                Product = product
            };

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var command = new DeleteProductCommand { ProductId = productId };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
