using Common.Core.CQRS.Commands;
using ProductManagementService.Data;
using ProductManagementService.Domain.Entities;
using System.Threading;

namespace ProductManagementService.Features.Products.Commands.Create
{
    public class CreateProductCommandHandler : CommandHandler<CreateProductCommand, int>
    {
        private readonly ProductManDbContext _dbContext;

        public CreateProductCommandHandler(ProductManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Product.Name,
                Description = request.Product.Description,
                OrganizationId = request.Product.OrganizationId
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
