using Common.Core.CQRS.Commands;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Commands.Update
{
    public class UpdateProductCommand : Command<bool>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
