using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ProductManagementService.Data;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Queries.Detail
{
    public class GetProductByIdQueryHandler : QueryHandler<GetProductByIdQuery, Product>
    {
        private readonly ProductManDbContext _dbContext;

        public GetProductByIdQueryHandler(ProductManDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product =  await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if(product == null)
                return null;

            return product;
        }
    }
}
