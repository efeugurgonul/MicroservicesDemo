using Common.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using ProductManagementService.Data;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Queries.List
{
    public class GetAllProductsQueryHandler : QueryHandler<GetAllProductsQuery, List<Product>>
    {
        private readonly ProductManDbContext _dBcontext;

        public GetAllProductsQueryHandler(ProductManDbContext dBcontext)
        {
            _dBcontext = dBcontext;
        }

        public override async Task<List<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _dBcontext.Products.AsQueryable();
            
            if (request.Filter.OrganizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == request.Filter.OrganizationId);
            }

            return await query.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OrganizationId = p.OrganizationId

            }).ToListAsync(cancellationToken);
        }
    }
}
