using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserID { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserRole { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // Login, SlipApproved, SlipRejected, etc.

        [MaxLength(500)]
        public string? Details { get; set; }

        [MaxLength(50)]
        public string? IPAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
