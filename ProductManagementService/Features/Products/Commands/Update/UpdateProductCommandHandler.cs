using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ProductManagementService.Data;

namespace ProductManagementService.Features.Products.Commands.Update
{
    public class UpdateProductCommandHandler : CommandHandler<UpdateProductCommand, bool>
    {
        private readonly ProductManDbContext _dbContext;

        public UpdateProductCommandHandler(ProductManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
                return false;

            product.Name = request.Product.Name;
            product.Description = request.Product.Description;
            product.OrganizationId = request.Product.OrganizationId;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
