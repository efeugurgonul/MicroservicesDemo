using System.ComponentModel.DataAnnotations;

namespace ServerManagementService.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        public int DefaultOrganizationId { get; set; }

        public bool IsActive { get; set; } = true;

        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();

        public List<UserOrganization> Organizations { get; set; } = new List<UserOrganization>();

        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
