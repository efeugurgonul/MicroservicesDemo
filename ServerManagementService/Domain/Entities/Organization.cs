using System.ComponentModel.DataAnnotations;

namespace ServerManagementService.Domain.Entities
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public int ActiveStatus { get; set; }
    }
}
