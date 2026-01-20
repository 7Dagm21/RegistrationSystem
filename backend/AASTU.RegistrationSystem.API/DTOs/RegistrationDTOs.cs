namespace AASTU.RegistrationSystem.API.DTOs
{
    public class CourseDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
    }

    public class CreateSlipRequest
    {
        public string StudentID { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public List<CourseDto> Courses { get; set; } = new();
    }

    public class SlipApprovalRequest
    {
        public int SlipId { get; set; }
        public bool IsApproved { get; set; }
        public string? Comment { get; set; }
    }

    public class RegistrationSlipDto
    {
        public int Id { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int AcademicYear { get; set; }
        public List<CourseDto> Courses { get; set; } = new();
        public int TotalCreditHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsAdvisorApproved { get; set; }
        public bool IsCostSharingVerified { get; set; }
        public bool IsRegistrarFinalized { get; set; }
        public string? QrCodeData { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CostSharingFormDto
    {
        public int Id { get; set; }
        public int RegistrationSlipId { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? PhotoDataUrl { get; set; }
        public string? PaymentInfo { get; set; }

        // Paper-form fields (student fills)
        public string? FullName { get; set; }
        public string? IdentityNo { get; set; }
        public string? Sex { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? MothersFullName { get; set; }
        public string? MothersAddress { get; set; }
        public string? SchoolName { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string? FacultyOrCollege { get; set; }
        public string? Department { get; set; }
        public string? EntranceYearEC { get; set; }
        public string? AcademicYearText { get; set; }
        public string? SemesterText { get; set; }

        // Costs & services (system + student selection)
        public decimal TuitionFee15Percent { get; set; }
        public decimal FoodExpense { get; set; }
        public decimal BoardingExpense { get; set; }
        public decimal TotalCost { get; set; }
        public string? ServiceSelection { get; set; }

        // Advance payment + signatures
        public DateTime? AdvancePaymentDate { get; set; }
        public string? Discount { get; set; }
        public string? ReceiptNo { get; set; }
        public string? BeneficiarySignatureName { get; set; }
        public DateTime? BeneficiarySignedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }

    public class SubmitCostSharingRequest
    {
        public int SlipId { get; set; }
        public string? PhotoPath { get; set; }
        public string? PhotoDataUrl { get; set; }
        public string? PaymentInfo { get; set; }

        // Paper-form fields (student fills)
        public string? FullName { get; set; }
        public string? IdentityNo { get; set; }
        public string? Sex { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? MothersFullName { get; set; }
        public string? MothersAddress { get; set; }
        public string? SchoolName { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string? FacultyOrCollege { get; set; }
        public string? Department { get; set; }
        public string? EntranceYearEC { get; set; }
        public string? AcademicYearText { get; set; }
        public string? SemesterText { get; set; }

        public string? ServiceSelection { get; set; }
        public DateTime? AdvancePaymentDate { get; set; }
        public string? Discount { get; set; }
        public string? ReceiptNo { get; set; }
        public string? BeneficiarySignatureName { get; set; }
        public DateTime? BeneficiarySignedAt { get; set; }
    }

    public class CourseGradeDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public decimal Credit { get; set; }
        public decimal NumberGrade { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public decimal GradePoint { get; set; }
    }

    public class CreateGradeReportRequest
    {
        public string StudentID { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty; // I, II
        public string AcademicYear { get; set; } = string.Empty; // 2022/2023
        public int Year { get; set; } // 1, 2, 3, 4, 5
        public string? Major { get; set; }
        public string? AdmissionClassification { get; set; }
        public string? Program { get; set; }
        public List<CourseGradeDto> Courses { get; set; } = new();
        public decimal? PreviousCredit { get; set; }
        public decimal? PreviousGP { get; set; }
        public decimal? PreviousANG { get; set; }
        public string? RegistrarRecorderName { get; set; }
        public DateTime? RegistrarSignedDate { get; set; }
        public string? Remark { get; set; }
    }

    public class GradeReportDto
    {
        public int Id { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public string? Major { get; set; }
        public string? AdmissionClassification { get; set; }
        public string? Program { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<CourseGradeDto> Courses { get; set; } = new();
        public decimal? PreviousCredit { get; set; }
        public decimal? PreviousGP { get; set; }
        public decimal? PreviousANG { get; set; }
        public decimal? SemesterCredit { get; set; }
        public decimal? SemesterGP { get; set; }
        public decimal? SemesterANG { get; set; }
        public decimal? CumulativeCredit { get; set; }
        public decimal? CumulativeGP { get; set; }
        public decimal? CumulativeANG { get; set; }
        public decimal? GPA { get; set; }
        public decimal? CGPA { get; set; }
        public string? Remark { get; set; }
        public string? RegistrarRecorderName { get; set; }
        public DateTime? RegistrarSignedDate { get; set; }
        public string? GeneratedBy { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GradeReportApprovalRequest
    {
        public bool IsApproved { get; set; }
        public string? Comment { get; set; }
    }
}
