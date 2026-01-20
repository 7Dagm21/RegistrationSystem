using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("RegistrationSlips")]
    public class RegistrationSlip
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string StudentID { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Semester { get; set; } = string.Empty; // e.g., "2024-2025 Fall"

        [Required]
        public int AcademicYear { get; set; } // Calculated: CurrentYear - EnrollmentYear + 1

        public string CoursesJson { get; set; } = "[]"; // JSON array of courses

        public int TotalCreditHours { get; set; }

        // Workflow status
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Created"; // Created, AdvisorApproved, CostSharingVerified, RegistrarFinalized

        // Approvals
        public bool IsAdvisorApproved { get; set; } = false;

        [MaxLength(50)]
        public string? AdvisorID { get; set; }

        [MaxLength(500)]
        public string? AdvisorComment { get; set; }

        public DateTime? AdvisorApprovedAt { get; set; }

        public bool IsCostSharingVerified { get; set; } = false;

        [MaxLength(50)]
        public string? CostSharingOfficerID { get; set; }

        public DateTime? CostSharingVerifiedAt { get; set; }

        public bool IsRegistrarFinalized { get; set; } = false;

        [MaxLength(50)]
        public string? RegistrarID { get; set; }

        public DateTime? RegistrarFinalizedAt { get; set; }

        // Generated documents
        public string? QrCodeData { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsLocked { get; set; } = false; // Locked after registrar finalization
    }
}
