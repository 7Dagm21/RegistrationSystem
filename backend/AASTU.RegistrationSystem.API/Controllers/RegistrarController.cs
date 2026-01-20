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
    [Authorize(Roles = "Registrar")]
    public class RegistrarController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public RegistrarController(IRegistrationService registrationService, ApplicationDbContext context, IPdfService pdfService)
        {
            _registrationService = registrationService;
            _context = context;
            _pdfService = pdfService;
        }

        [HttpGet("pending-approvals")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slips = await _registrationService.GetPendingApprovalsAsync("Registrar", userId);
            return Ok(slips);
        }

        [HttpGet("slips/{id}")]
        public async Task<IActionResult> GetSlip(int id)
        {
            var slip = await _registrationService.GetSlipByIdAsync(id);
            if (slip == null)
            {
                return NotFound();
            }

            var costSharingForm = await _context.CostSharingForms.FirstOrDefaultAsync(c => c.RegistrationSlipId == id);
            var gradeReports = await _context.GradeReports
                .Where(g => g.StudentID == slip.StudentID)
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                slip = slip,
                costSharingForm = costSharingForm,
                recentGradeReports = gradeReports
            });
        }

        [HttpPost("slips/{id}/finalize")]
        public async Task<IActionResult> FinalizeRegistration(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _registrationService.FinalizeRegistrationAsync(id, userId);

            if (result)
            {
                return Ok(new { message = "Registration finalized successfully" });
            }

            return BadRequest(new { message = "Failed to finalize registration" });
        }

        [HttpGet("slips/{id}/pdf")]
        public async Task<IActionResult> GenerateSlipPdf(int id)
        {
            var slip = await _registrationService.GetSlipByIdAsync(id);
            if (slip == null || !slip.IsRegistrarFinalized)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateApprovedSlipPdfAsync(slip);
            return File(pdfBytes, "application/pdf", $"RegistrationSlip_{slip.SerialNumber}.pdf");
        }

        [HttpGet("students/search")]
        public async Task<IActionResult> SearchStudent([FromQuery] string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return BadRequest(new { message = "Student ID is required" });
            }

            // Trim the input
            studentId = studentId.Trim();

            // Try exact match
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null)
            {
                // Get total count and sample IDs for debugging
                var totalCount = await _context.Students.CountAsync();
                var sampleIds = await _context.Students.Select(s => s.StudentID).Take(10).ToListAsync();
                
                return NotFound(new { 
                    message = $"Student with ID '{studentId}' not found",
                    searchedId = studentId,
                    totalStudentsInDatabase = totalCount,
                    sampleIds = sampleIds,
                    hint = totalCount == 0 
                        ? "No students found in database. Please seed the database first."
                        : "Student IDs are in format: ETS####/YY (e.g., ETS0358/15). Make sure you enter the exact ID."
                });
            }

            int academicYear = await _registrationService.CalculateAcademicYear(student.StudentID);

            // Get ALL student's registration slips - show all with their status
            // The frontend will filter/display appropriately
            var slips = await _registrationService.GetStudentSlipsAsync(studentId);
            
            // Sort: finalized first, then by creation date
            var sortedSlips = slips
                .OrderByDescending(s => s.IsRegistrarFinalized) // Finalized first
                .ThenByDescending(s => s.IsCostSharingVerified)
                .ThenByDescending(s => s.IsAdvisorApproved)
                .ThenByDescending(s => s.CreatedAt)
                .ToList();

            return Ok(new
            {
                studentId = student.StudentID,
                fullName = student.FullName,
                email = student.UniversityEmail,
                department = student.Department,
                enrollmentYear = student.EnrollmentYear,
                academicYear = academicYear,
                registrationSlips = sortedSlips,
                totalSlips = sortedSlips.Count,
                approvedSlips = sortedSlips.Count(s => s.IsAdvisorApproved),
                finalizedSlips = sortedSlips.Count(s => s.IsRegistrarFinalized)
            });
        }

        [HttpGet("archive")]
        public async Task<IActionResult> GetArchive([FromQuery] string? studentId, [FromQuery] string? semester, [FromQuery] string? department)
        {
            var query = _context.RegistrationSlips.AsQueryable();

            if (!string.IsNullOrEmpty(studentId))
            {
                query = query.Where(s => s.StudentID == studentId);
            }

            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(s => s.Semester == semester);
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(s => s.Department == department);
            }

            var slips = await query
                .Where(s => s.IsRegistrarFinalized)
                .OrderByDescending(s => s.CreatedAt)
                .Take(100)
                .ToListAsync();

            var result = slips.Select(s => new RegistrationSlipDto
            {
                Id = s.Id,
                StudentID = s.StudentID,
                StudentName = s.StudentName,
                Department = s.Department,
                Semester = s.Semester,
                AcademicYear = s.AcademicYear,
                TotalCreditHours = s.TotalCreditHours,
                Status = s.Status,
                SerialNumber = s.SerialNumber,
                CreatedAt = s.CreatedAt
            }).ToList();

            return Ok(result);
        }
    }
}
