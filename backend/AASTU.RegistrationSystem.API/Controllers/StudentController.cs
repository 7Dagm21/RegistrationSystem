using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Data;

namespace AASTU.RegistrationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public StudentController(IRegistrationService registrationService, ApplicationDbContext context, IPdfService pdfService)
        {
            _registrationService = registrationService;
            _context = context;
            _pdfService = pdfService;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHome()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == userId);

            if (student == null)
            {
                return NotFound();
            }

            int academicYear = await _registrationService.CalculateAcademicYear(userId);
            var slips = await _registrationService.GetStudentSlipsAsync(userId);
            var latestSlip = slips.FirstOrDefault();

            return Ok(new
            {
                studentId = student.StudentID,
                fullName = student.FullName,
                email = student.UniversityEmail,
                department = student.Department,
                enrollmentYear = student.EnrollmentYear,
                academicYear = academicYear,
                registrationStatus = latestSlip?.Status ?? "Not Registered",
                latestSlip = latestSlip
            });
        }

        [HttpGet("slips")]
        public async Task<IActionResult> GetSlips()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slips = await _registrationService.GetStudentSlipsAsync(userId);
            return Ok(slips);
        }

        [HttpGet("slips/{id}")]
        public async Task<IActionResult> GetSlip(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.GetSlipByIdAsync(id);

            if (slip == null || slip.StudentID != userId)
            {
                return NotFound();
            }

            return Ok(slip);
        }

        [HttpGet("slips/{id}/approved-slip")]
        public async Task<IActionResult> GetApprovedSlip(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.GetSlipByIdAsync(id);

            if (slip == null || slip.StudentID != userId || !slip.IsRegistrarFinalized)
            {
                return NotFound();
            }

            return Ok(slip);
        }

        [HttpGet("slips/{id}/pdf")]
        public async Task<IActionResult> DownloadSlipPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.GetSlipByIdAsync(id);

            if (slip == null || slip.StudentID != userId || !slip.IsRegistrarFinalized)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateApprovedSlipPdfAsync(slip);
            return File(pdfBytes, "application/pdf", $"RegistrationSlip_{slip.SerialNumber}.pdf");
        }

        [HttpGet("progress/{slipId}")]
        public async Task<IActionResult> GetProgress(int slipId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.GetSlipByIdAsync(slipId);

            if (slip == null || slip.StudentID != userId)
            {
                return NotFound();
            }

            var steps = new
            {
                slipCreated = true,
                advisorApproved = slip.IsAdvisorApproved,
                costSharingVerified = slip.IsCostSharingVerified,
                registrarFinalized = slip.IsRegistrarFinalized
            };

            return Ok(steps);
        }

        [HttpGet("slips/{id}/cost-sharing-form")]
        public async Task<IActionResult> GetCostSharingForm(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.GetSlipByIdAsync(id);

            if (slip == null || slip.StudentID != userId || !slip.IsAdvisorApproved)
            {
                return NotFound();
            }

            var costSharingForm = await _context.CostSharingForms
                .FirstOrDefaultAsync(c => c.RegistrationSlipId == id);

            if (costSharingForm == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = costSharingForm.Id,
                registrationSlipId = costSharingForm.RegistrationSlipId,
                studentId = costSharingForm.StudentID,
                tuitionFee15Percent = costSharingForm.TuitionFee15Percent,
                foodExpense = costSharingForm.FoodExpense,
                boardingExpense = costSharingForm.BoardingExpense,
                totalCost = costSharingForm.TotalCost,
                status = costSharingForm.Status,
                createdAt = costSharingForm.CreatedAt
            });
        }

        [HttpGet("slips/{id}/cost-sharing-form/pdf")]
        public async Task<IActionResult> DownloadCostSharingFormPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _context.RegistrationSlips.FirstOrDefaultAsync(s => s.Id == id);

            if (slip == null || slip.StudentID != userId || !slip.IsCostSharingVerified)
            {
                return NotFound();
            }

            var costSharingForm = await _context.CostSharingForms
                .FirstOrDefaultAsync(c => c.RegistrationSlipId == id);

            if (costSharingForm == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == userId);
            if (student == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateCostSharingFormPdfAsync(costSharingForm, student, slip);
            return File(pdfBytes, "application/pdf", $"CostSharingForm_{slip.StudentID}_{id}.pdf");
        }

        [HttpGet("grade-reports")]
        public async Task<IActionResult> GetGradeReports()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gradeReports = await _context.GradeReports
                .Where(g => g.StudentID == userId && g.Status == "DepartmentHeadApproved")
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var result = gradeReports.Select(g =>
            {
                var courses = System.Text.Json.JsonSerializer.Deserialize<List<CourseGradeDto>>(g.GradesJson) ?? new List<CourseGradeDto>();
                return new GradeReportDto
                {
                    Id = g.Id,
                    StudentID = g.StudentID,
                    StudentName = g.StudentName,
                    Major = g.Major,
                    Semester = g.Semester,
                    AcademicYear = g.AcademicYear,
                    Year = g.Year,
                    Courses = courses,
                    SemesterCredit = g.SemesterCredit,
                    SemesterGP = g.SemesterGP,
                    SemesterANG = g.SemesterANG,
                    CumulativeCredit = g.CumulativeCredit,
                    CumulativeGP = g.CumulativeGP,
                    CumulativeANG = g.CumulativeANG,
                    GPA = g.GPA,
                    CGPA = g.CGPA,
                    Remark = g.Remark,
                    Status = g.Status,
                    CreatedAt = g.CreatedAt
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("cost-sharing-forms")]
        public async Task<IActionResult> GetAllCostSharingForms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var forms = await _context.CostSharingForms
                .Where(f => f.StudentID == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var result = forms.Select(f => new {
                id = f.Id,
                registrationSlipId = f.RegistrationSlipId,
                studentId = f.StudentID,
                tuitionFee15Percent = f.TuitionFee15Percent,
                foodExpense = f.FoodExpense,
                boardingExpense = f.BoardingExpense,
                totalCost = f.TotalCost,
                status = f.Status,
                submittedAt = f.SubmittedAt,
                createdAt = f.CreatedAt
            }).ToList();

            return Ok(result);
        }

        [HttpGet("grade-reports/{id}/pdf")]
        public async Task<IActionResult> DownloadGradeReportPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gradeReport = await _context.GradeReports.FirstOrDefaultAsync(g => g.Id == id && g.StudentID == userId);
            if (gradeReport == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateGradeReportPdfAsync(gradeReport);
            return File(pdfBytes, "application/pdf", $"GradeReport_{gradeReport.StudentID}_{id}.pdf");
        }
    }
}
