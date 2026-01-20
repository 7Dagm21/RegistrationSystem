using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AASTU.RegistrationSystem.API.Data;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQrCodeService _qrCodeService;
        private readonly IAuditLogService _auditLogService;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public RegistrationService(ApplicationDbContext context, IQrCodeService qrCodeService, IAuditLogService auditLogService, IPdfService pdfService, IEmailService emailService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _auditLogService = auditLogService;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        public async Task<RegistrationSlipDto?> CreateSlipAsync(CreateSlipRequest request, string departmentHeadId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == request.StudentID);
            if (student == null)
            {
                return null;
            }

            int academicYear = await CalculateAcademicYearAsync(request.StudentID);

            var slip = new RegistrationSlip
            {
                StudentID = request.StudentID,
                StudentName = student.FullName,
                Department = student.Department,
                Semester = request.Semester,
                AcademicYear = academicYear,
                CoursesJson = JsonSerializer.Serialize(request.Courses),
                TotalCreditHours = request.Courses.Sum(c => c.CreditHours),
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            _context.RegistrationSlips.Add(slip);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(departmentHeadId, "DepartmentHead", "SlipCreated", $"Created slip for student {request.StudentID}", null);

            return MapToDto(slip);
        }

        public async Task<RegistrationSlipDto?> CreateSlipByAdvisorAsync(CreateSlipRequest request, string advisorId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == request.StudentID);
            if (student == null)
            {
                return null;
            }

            int academicYear = await CalculateAcademicYearAsync(request.StudentID);

            var slip = new RegistrationSlip
            {
                StudentID = request.StudentID,
                StudentName = student.FullName,
                Department = student.Department,
                Semester = request.Semester,
                AcademicYear = academicYear,
                CoursesJson = JsonSerializer.Serialize(request.Courses),
                TotalCreditHours = request.Courses.Sum(c => c.CreditHours),
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            _context.RegistrationSlips.Add(slip);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(advisorId, "Advisor", "SlipCreated", $"Advisor created slip for student {request.StudentID}", null);

            return MapToDto(slip);
        }

        public async Task<List<RegistrationSlipDto>> GetStudentSlipsAsync(string studentId)
        {
            var slips = await _context.RegistrationSlips
                .Where(s => s.StudentID == studentId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return slips.Select(MapToDto).ToList();
        }

        public async Task<RegistrationSlipDto?> GetSlipByIdAsync(int slipId)
        {
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == slipId);
            return slip != null ? MapToDto(slip) : null;
        }

        public async Task<bool> ApproveSlipAsync(int slipId, string advisorId, string? comment)
        {
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == slipId);
            if (slip == null || slip.Status != "Created")
            {
                return false;
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == slip.StudentID);
            if (student == null)
            {
                return false;
            }

            slip.IsAdvisorApproved = true;
            slip.AdvisorID = advisorId;
            slip.AdvisorComment = comment;
            slip.AdvisorApprovedAt = DateTime.UtcNow;
            slip.Status = "AdvisorApproved";
            slip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Cost-sharing amounts (STATIC as requested)
            decimal tuitionFee15Percent = 1382.11m;
            decimal foodExpense = 22980.00m;
            decimal boardingExpense = 600.00m;
            decimal totalCost = 24962.11m;

            // Create cost-sharing form
            var costSharingForm = new CostSharingForm
            {
                RegistrationSlipId = slipId,
                StudentID = slip.StudentID,
                TuitionFee15Percent = tuitionFee15Percent,
                FoodExpense = foodExpense,
                BoardingExpense = boardingExpense,
                TotalCost = totalCost,
                ServiceSelection = JsonSerializer.Serialize(new { inKind = "Boarding only", inCash = "Food only" }),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.CostSharingForms.Add(costSharingForm);
            await _context.SaveChangesAsync();

            // Generate PDF
            var pdfBytes = await _pdfService.GenerateCostSharingFormPdfAsync(costSharingForm, student, slip);

            // Send email to student with the form attached
            var emailSubject = "AASTU Registration - Cost-Sharing Form";
            var emailBody = $@"
                <html>
                <body>
                    <h2>Registration Slip Approved</h2>
                    <p>Dear {student.FullName},</p>
                    <p>Your registration slip has been approved by your advisor.</p>
                    <p>Please find attached your Cost-Sharing Beneficiaries Agreement Form. This form must be completed and submitted to proceed with your registration.</p>
                    <p><strong>Important:</strong></p>
                    <ul>
                        <li>Fill out all required sections of the form</li>
                        <li>Attach a passport-size photo</li>
                        <li>Submit the completed form to the Cost-Sharing Office</li>
                    </ul>
                    <p>If you have any questions, please contact the Registrar's Office.</p>
                    <br/>
                    <p>Best regards,<br/>AASTU Registration System</p>
                </body>
                </html>";

            await _emailService.SendEmailWithAttachmentAsync(
                student.UniversityEmail,
                emailSubject,
                emailBody,
                pdfBytes,
                $"CostSharingForm_{slip.StudentID}_{slipId}.pdf"
            );

            await _auditLogService.LogAsync(advisorId, "Advisor", "SlipApproved", $"Approved slip {slipId} for student {slip.StudentID} and sent cost-sharing form", null);

            return true;
        }

        public async Task<bool> RejectSlipAsync(int slipId, string advisorId, string comment)
        {
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == slipId);
            if (slip == null || slip.Status != "Created")
            {
                return false;
            }

            slip.Status = "Rejected";
            slip.AdvisorID = advisorId;
            slip.AdvisorComment = comment;
            slip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(advisorId, "Advisor", "SlipRejected", $"Rejected slip {slipId}: {comment}", null);

            return true;
        }

        public async Task<bool> VerifyCostSharingAsync(int slipId, string officerId)
        {
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == slipId);
            if (slip == null || slip.Status != "AdvisorApproved")
            {
                return false;
            }

            var costSharingForm = await _context.CostSharingForms.FirstOrDefaultAsync(c => c.RegistrationSlipId == slipId);
            if (costSharingForm == null || costSharingForm.Status != "Pending")
            {
                return false;
            }

            slip.IsCostSharingVerified = true;
            slip.CostSharingOfficerID = officerId;
            slip.CostSharingVerifiedAt = DateTime.UtcNow;
            slip.Status = "CostSharingVerified";
            slip.UpdatedAt = DateTime.UtcNow;

            costSharingForm.Status = "Verified";
            costSharingForm.VerifiedBy = officerId;
            costSharingForm.VerifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(officerId, "CostSharingOfficer", "CostSharingVerified", $"Verified cost sharing for slip {slipId}", null);

            return true;
        }

        public async Task<bool> FinalizeRegistrationAsync(int slipId, string registrarId)
        {
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == slipId);
            if (slip == null || slip.Status != "CostSharingVerified")
            {
                return false;
            }

            slip.IsRegistrarFinalized = true;
            slip.RegistrarID = registrarId;
            slip.RegistrarFinalizedAt = DateTime.UtcNow;
            slip.Status = "RegistrarFinalized";
            slip.IsLocked = true;
            slip.UpdatedAt = DateTime.UtcNow;

            // Generate QR Code and Serial Number
            slip.SerialNumber = GenerateSerialNumber();
            slip.QrCodeData = await _qrCodeService.GenerateQrCodeAsync(slip.SerialNumber, slip.StudentID, slip.Semester);

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(registrarId, "Registrar", "RegistrationFinalized", $"Finalized registration slip {slipId} for student {slip.StudentID}", null);

            return true;
        }

        public async Task<List<RegistrationSlipDto>> GetPendingApprovalsAsync(string role, string userId)
        {
            IQueryable<RegistrationSlip> query = _context.RegistrationSlips;

            switch (role)
            {
                case "Advisor":
                    query = query.Where(s => s.Status == "Created");
                    break;
                case "CostSharingOfficer":
                    query = query.Where(s => s.Status == "AdvisorApproved");
                    break;
                case "Registrar":
                    query = query.Where(s => s.Status == "CostSharingVerified");
                    break;
                default:
                    return new List<RegistrationSlipDto>();
            }

            var slips = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
            return slips.Select(MapToDto).ToList();
        }

        public async Task<List<CourseDto>> GetCoursesByYearAsync(int academicYear, string department)
        {
            var courses = await _context.Courses
                .Where(c => c.AcademicYear == academicYear && c.Department == department)
                .ToListAsync();

            return courses.Select(c => new CourseDto
            {
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                CreditHours = c.CreditHours
            }).ToList();
        }

        public async Task<List<CourseDto>> GetCoursesAsync(int academicYear, string department, string? semester)
        {
            var query = _context.Courses.AsQueryable();
            query = query.Where(c => c.AcademicYear == academicYear && c.Department == department);
            if (!string.IsNullOrWhiteSpace(semester))
            {
                query = query.Where(c => c.Semester == semester);
            }

            var courses = await query.ToListAsync();

            return courses.Select(c => new CourseDto
            {
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                CreditHours = c.CreditHours
            }).ToList();
        }

        public async Task<int> CalculateAcademicYear(string studentId)
        {
            return await CalculateAcademicYearAsync(studentId);
        }

        private async Task<int> CalculateAcademicYearAsync(string studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == studentId);
            if (student == null)
            {
                return 1;
            }

            int currentYear = DateTime.Now.Year;
            int enrollmentYear = student.EnrollmentYear;
            int academicYear = currentYear - enrollmentYear + 1;

            return Math.Max(1, Math.Min(academicYear, 5)); // Clamp between 1 and 5
        }

        private RegistrationSlipDto MapToDto(RegistrationSlip slip)
        {
            var courses = JsonSerializer.Deserialize<List<CourseDto>>(slip.CoursesJson) ?? new List<CourseDto>();

            return new RegistrationSlipDto
            {
                Id = slip.Id,
                StudentID = slip.StudentID,
                StudentName = slip.StudentName,
                Department = slip.Department,
                Semester = slip.Semester,
                AcademicYear = slip.AcademicYear,
                Courses = courses,
                TotalCreditHours = slip.TotalCreditHours,
                Status = slip.Status,
                IsAdvisorApproved = slip.IsAdvisorApproved,
                IsCostSharingVerified = slip.IsCostSharingVerified,
                IsRegistrarFinalized = slip.IsRegistrarFinalized,
                QrCodeData = slip.QrCodeData,
                SerialNumber = slip.SerialNumber,
                CreatedAt = slip.CreatedAt
            };
        }

        private string GenerateSerialNumber()
        {
            return $"AASTU-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
