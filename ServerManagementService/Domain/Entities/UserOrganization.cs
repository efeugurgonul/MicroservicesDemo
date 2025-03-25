using ServerManagementService.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServerManagementService.Domain.Entities
{
    public class UserOrganization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; } = null!;
    }
}
