using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("CostSharingForms")]
    public class CostSharingForm
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RegistrationSlipId { get; set; }

        [Required]
        [MaxLength(50)]
        public string StudentID { get; set; } = string.Empty;

        // Student photo (either a URL/path or a data URL from upload)
        [MaxLength(500)]
        public string? PhotoPath { get; set; }

        public string? PhotoDataUrl { get; set; } // "data:image/...;base64,...."

        [MaxLength(500)]
        public string? PaymentInfo { get; set; } // JSON or text

        // ===== Paper-form fields (digital form data) =====
        [MaxLength(200)]
        public string? FullName { get; set; } // including grandfather's name

        [MaxLength(50)]
        public string? IdentityNo { get; set; } // can be ETS id / StudentID / gov id

        [MaxLength(10)]
        public string? Sex { get; set; } // Male/Female

        [MaxLength(50)]
        public string? Nationality { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? PlaceOfBirth { get; set; } // free text (Region/Zone/Woreda/Town/Kebele)

        [MaxLength(200)]
        public string? MothersFullName { get; set; }

        [MaxLength(500)]
        public string? MothersAddress { get; set; }

        [MaxLength(200)]
        public string? SchoolName { get; set; } // preparatory

        public DateTime? DateCompleted { get; set; }

        [MaxLength(200)]
        public string? FacultyOrCollege { get; set; }

        [MaxLength(200)]
        public string? Department { get; set; }

        [MaxLength(50)]
        public string? EntranceYearEC { get; set; }

        [MaxLength(50)]
        public string? AcademicYearText { get; set; } // e.g. "IV"

        [MaxLength(50)]
        public string? SemesterText { get; set; } // e.g. "I"

        // Cost calculation fields
        public decimal TuitionFee15Percent { get; set; }
        public decimal FoodExpense { get; set; }
        public decimal BoardingExpense { get; set; }
        public decimal TotalCost { get; set; }

        // Service selection (stored as JSON)
        [MaxLength(500)]
        public string? ServiceSelection { get; set; } // JSON: { "inKind": "Boarding only", "inCash": "Food only" }

        // Advance payment info
        public DateTime? AdvancePaymentDate { get; set; }

        [MaxLength(100)]
        public string? Discount { get; set; }

        [MaxLength(100)]
        public string? ReceiptNo { get; set; }

        // Signatures
        [MaxLength(200)]
        public string? BeneficiarySignatureName { get; set; }

        public DateTime? BeneficiarySignedAt { get; set; }

        [MaxLength(200)]
        public string? InstituteRepresentativeName { get; set; }

        public DateTime? InstituteSignedAt { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected

        [MaxLength(50)]
        public string? VerifiedBy { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? PdfPath { get; set; } // Path to generated PDF
    }
}
