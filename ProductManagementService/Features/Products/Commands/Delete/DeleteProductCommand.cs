using Common.Core.CQRS.Commands;

namespace ProductManagementService.Features.Products.Commands.Delete
{
    public class DeleteProductCommand : Command<bool>
    {
        public int ProductId { get; set; }
    }
}
