using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("GradeReports")]
    public class GradeReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string StudentID { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? StudentName { get; set; }

        [MaxLength(100)]
        public string? Major { get; set; }

        [MaxLength(50)]
        public string? AdmissionClassification { get; set; } // Regular, Extension, etc.

        [MaxLength(50)]
        public string? Program { get; set; } // Degree, etc.

        [Required]
        public int Year { get; set; } // 1, 2, 3, 4, 5

        [Required]
        [MaxLength(20)]
        public string Semester { get; set; } = string.Empty; // I, II

        [Required]
        [MaxLength(50)]
        public string AcademicYear { get; set; } = string.Empty; // 2022/2023

        public string GradesJson { get; set; } = "[]"; // JSON array of course grades

        // Previous totals
        public decimal? PreviousCredit { get; set; }
        public decimal? PreviousGP { get; set; }
        public decimal? PreviousANG { get; set; }

        // Semester totals
        public decimal? SemesterCredit { get; set; }
        public decimal? SemesterGP { get; set; }
        public decimal? SemesterANG { get; set; }

        // Cumulative totals
        public decimal? CumulativeCredit { get; set; }
        public decimal? CumulativeGP { get; set; }
        public decimal? CumulativeANG { get; set; }

        public decimal? GPA { get; set; }

        public decimal? CGPA { get; set; }

        [MaxLength(50)]
        public string? Remark { get; set; } // X Promoted, Academic Warning, etc.

        [MaxLength(200)]
        public string? RegistrarRecorderName { get; set; }

        public DateTime? RegistrarSignedDate { get; set; }

        [MaxLength(200)]
        public string? GeneratedBy { get; set; }

        public DateTime? GeneratedAt { get; set; }

        // Approval workflow
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Created"; // Created, DepartmentHeadApproved, Rejected

        [MaxLength(50)]
        public string? ApprovedBy { get; set; } // Department Head ID

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
