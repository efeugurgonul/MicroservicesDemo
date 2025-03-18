using Common.Core.CQRS.Queries;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Queries.Detail
{
    public class GetProductByIdQuery : Query<Product>
    {
        public int Id { get; set; }        
    }
}
