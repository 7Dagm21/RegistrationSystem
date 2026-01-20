using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("Staff")]
    public class Staff
    {
        [Key]
        [MaxLength(50)]
        public string StaffID { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty; // Advisor, DepartmentHead, Registrar, CostSharingOfficer, SystemAdmin
    }
}
