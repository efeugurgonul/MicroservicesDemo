using Common.Core.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using ProductManagementService.Data;

namespace ProductManagementService.Features.Products.Commands.Delete
{
    public class DeleteProductCommandHandler : CommandHandler<DeleteProductCommand, bool>
    {
        private readonly ProductManDbContext _dbContext;

        public DeleteProductCommandHandler(ProductManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
                return false;

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
