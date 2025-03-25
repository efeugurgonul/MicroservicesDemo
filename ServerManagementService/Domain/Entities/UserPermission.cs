using Common.Core.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServerManagementService.Domain.Entities
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public Permission Permission { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
