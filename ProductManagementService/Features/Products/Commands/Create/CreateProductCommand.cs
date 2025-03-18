using Common.Core.CQRS.Commands;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Features.Products.Commands.Create
{
    public class CreateProductCommand : Command<int>
    {
        public required Product Product { get; set; }
    }
}
