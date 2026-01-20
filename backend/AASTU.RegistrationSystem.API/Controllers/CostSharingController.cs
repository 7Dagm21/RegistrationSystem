using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Data;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CostSharingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public CostSharingController(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitCostSharing([FromBody] SubmitCostSharingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == request.SlipId && s.StudentID == userId);

            if (slip == null)
            {
                return NotFound();
            }

            if (slip.Status != "AdvisorApproved")
            {
                return BadRequest(new { message = "Slip must be approved by advisor first" });
            }

            var existingForm = await _context.CostSharingForms.FirstOrDefaultAsync(c => c.RegistrationSlipId == request.SlipId);
            if (existingForm != null)
            {
                existingForm.PhotoPath = request.PhotoPath;
                existingForm.PhotoDataUrl = request.PhotoDataUrl;
                existingForm.PaymentInfo = request.PaymentInfo;

                // Paper-form fields
                existingForm.FullName = request.FullName;
                existingForm.IdentityNo = request.IdentityNo;
                existingForm.Sex = request.Sex;
                existingForm.Nationality = request.Nationality;
                existingForm.DateOfBirth = request.DateOfBirth;
                existingForm.PlaceOfBirth = request.PlaceOfBirth;
                existingForm.MothersFullName = request.MothersFullName;
                existingForm.MothersAddress = request.MothersAddress;
                existingForm.SchoolName = request.SchoolName;
                existingForm.DateCompleted = request.DateCompleted;
                existingForm.FacultyOrCollege = request.FacultyOrCollege;
                existingForm.Department = request.Department;
                existingForm.EntranceYearEC = request.EntranceYearEC;
                existingForm.AcademicYearText = request.AcademicYearText;
                existingForm.SemesterText = request.SemesterText;

                // Selection + advance payment + signature
                existingForm.ServiceSelection = request.ServiceSelection ?? existingForm.ServiceSelection;
                existingForm.AdvancePaymentDate = request.AdvancePaymentDate;
                existingForm.Discount = request.Discount;
                existingForm.ReceiptNo = request.ReceiptNo;
                existingForm.BeneficiarySignatureName = request.BeneficiarySignatureName;
                existingForm.BeneficiarySignedAt = request.BeneficiarySignedAt;

                existingForm.Status = "Pending";
                existingForm.SubmittedAt = DateTime.UtcNow;
            }
            else
            {
                var form = new CostSharingForm
                {
                    RegistrationSlipId = request.SlipId,
                    StudentID = userId,
                    PhotoPath = request.PhotoPath,
                    PhotoDataUrl = request.PhotoDataUrl,
                    PaymentInfo = request.PaymentInfo,

                    // Paper-form fields
                    FullName = request.FullName,
                    IdentityNo = request.IdentityNo,
                    Sex = request.Sex,
                    Nationality = request.Nationality,
                    DateOfBirth = request.DateOfBirth,
                    PlaceOfBirth = request.PlaceOfBirth,
                    MothersFullName = request.MothersFullName,
                    MothersAddress = request.MothersAddress,
                    SchoolName = request.SchoolName,
                    DateCompleted = request.DateCompleted,
                    FacultyOrCollege = request.FacultyOrCollege,
                    Department = request.Department,
                    EntranceYearEC = request.EntranceYearEC,
                    AcademicYearText = request.AcademicYearText,
                    SemesterText = request.SemesterText,

                    ServiceSelection = request.ServiceSelection,
                    AdvancePaymentDate = request.AdvancePaymentDate,
                    Discount = request.Discount,
                    ReceiptNo = request.ReceiptNo,
                    BeneficiarySignatureName = request.BeneficiarySignatureName,
                    BeneficiarySignedAt = request.BeneficiarySignedAt,

                    Status = "Pending",
                    SubmittedAt = DateTime.UtcNow
                };
                _context.CostSharingForms.Add(form);
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(userId, "Student", "CostSharingSubmitted", $"Submitted cost sharing form for slip {request.SlipId}", null);

            return Ok(new { message = "Cost sharing form submitted successfully" });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "CostSharingOfficer")]
        public async Task<IActionResult> GetPendingPayments()
        {
            var forms = await _context.CostSharingForms
                .Where(c => c.Status == "Pending")
                .ToListAsync();

            var result = forms.Select(f => new CostSharingFormDto
            {
                Id = f.Id,
                RegistrationSlipId = f.RegistrationSlipId,
                StudentID = f.StudentID,
                PhotoPath = f.PhotoPath,
                PhotoDataUrl = null, // don't send large blobs in list
                PaymentInfo = f.PaymentInfo,
                FullName = f.FullName,
                Sex = f.Sex,
                Nationality = f.Nationality,
                Status = f.Status,
                SubmittedAt = f.SubmittedAt
            }).ToList();

            return Ok(result);
        }

        [HttpGet("review/{id}")]
        [Authorize(Roles = "CostSharingOfficer")]
        public async Task<IActionResult> ReviewCostSharing(int id)
        {
            var form = await _context.CostSharingForms.FirstOrDefaultAsync(c => c.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == form.RegistrationSlipId);

            return Ok(new
            {
                form = new CostSharingFormDto
                {
                    Id = form.Id,
                    RegistrationSlipId = form.RegistrationSlipId,
                    StudentID = form.StudentID,
                    PhotoPath = form.PhotoPath,
                    PhotoDataUrl = form.PhotoDataUrl,
                    PaymentInfo = form.PaymentInfo,
                    FullName = form.FullName,
                    IdentityNo = form.IdentityNo,
                    Sex = form.Sex,
                    Nationality = form.Nationality,
                    DateOfBirth = form.DateOfBirth,
                    PlaceOfBirth = form.PlaceOfBirth,
                    MothersFullName = form.MothersFullName,
                    MothersAddress = form.MothersAddress,
                    SchoolName = form.SchoolName,
                    DateCompleted = form.DateCompleted,
                    FacultyOrCollege = form.FacultyOrCollege,
                    Department = form.Department,
                    EntranceYearEC = form.EntranceYearEC,
                    AcademicYearText = form.AcademicYearText,
                    SemesterText = form.SemesterText,
                    TuitionFee15Percent = form.TuitionFee15Percent,
                    FoodExpense = form.FoodExpense,
                    BoardingExpense = form.BoardingExpense,
                    TotalCost = form.TotalCost,
                    ServiceSelection = form.ServiceSelection,
                    AdvancePaymentDate = form.AdvancePaymentDate,
                    Discount = form.Discount,
                    ReceiptNo = form.ReceiptNo,
                    BeneficiarySignatureName = form.BeneficiarySignatureName,
                    BeneficiarySignedAt = form.BeneficiarySignedAt,
                    Status = form.Status,
                    SubmittedAt = form.SubmittedAt
                },
                slip = slip
            });
        }

        [HttpPost("verify/{id}")]
        [Authorize(Roles = "CostSharingOfficer")]
        public async Task<IActionResult> VerifyCostSharing(int id, [FromServices] IRegistrationService registrationService)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var form = await _context.CostSharingForms.FirstOrDefaultAsync(c => c.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            var result = await registrationService.VerifyCostSharingAsync(form.RegistrationSlipId, userId);
            if (result)
            {
                return Ok(new { message = "Cost sharing verified successfully" });
            }

            return BadRequest(new { message = "Failed to verify cost sharing" });
        }
    }
}
