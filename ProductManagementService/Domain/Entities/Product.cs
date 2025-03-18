using System.ComponentModel.DataAnnotations;

namespace ProductManagementService.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public int OrganizationId { get; set; }
    }
}
