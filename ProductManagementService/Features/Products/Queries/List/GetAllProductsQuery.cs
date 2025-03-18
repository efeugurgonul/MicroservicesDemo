using Common.Core.CQRS.Queries;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Queries.List
{
    public class ProductFilter
    {       
        public int? OrganizationId { get; set; }
    }

    public class GetAllProductsQuery : Query<List<Product>>
    {
        public ProductFilter Filter { get; set; } = new ProductFilter();
    }
}
