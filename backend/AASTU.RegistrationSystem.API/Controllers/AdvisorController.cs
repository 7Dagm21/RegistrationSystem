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
    [Authorize(Roles = "Advisor")]
    public class AdvisorController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ApplicationDbContext _context;

        public AdvisorController(IRegistrationService registrationService, ApplicationDbContext context)
        {
            _registrationService = registrationService;
            _context = context;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHome()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingSlips = await _registrationService.GetPendingApprovalsAsync("Advisor", userId);

            // Get assigned students (simplified - in real system, you'd have an assignment table)
            var students = await _context.Students
                .Where(s => s.Department == User.FindFirstValue("Department") || true) // Simplified
                .Take(50)
                .ToListAsync();

            return Ok(new
            {
                advisorId = userId,
                pendingApprovals = pendingSlips.Count,
                assignedStudents = students.Select(s => new { s.StudentID, s.FullName, s.Department }),
                pendingSlips = pendingSlips
            });
        }

        [HttpGet("pending-slips")]
        public async Task<IActionResult> GetPendingSlips()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slips = await _registrationService.GetPendingApprovalsAsync("Advisor", userId);
            return Ok(slips);
        }

        // Advisor creates slip from course catalog (year + semester + department)
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] int academicYear, [FromQuery] string department, [FromQuery] string? semester)
        {
            var courses = await _registrationService.GetCoursesAsync(academicYear, department, semester);
            return Ok(courses);
        }

        [HttpPost("slips/create")]
        public async Task<IActionResult> CreateSlip([FromBody] CreateSlipRequest request)
        {
            var advisorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(advisorId))
            {
                return Unauthorized();
            }

            if (request.Courses == null || request.Courses.Count == 0)
            {
                return BadRequest(new { message = "Select at least one course" });
            }

            var slip = await _registrationService.CreateSlipByAdvisorAsync(request, advisorId);
            if (slip == null)
            {
                return BadRequest(new { message = "Failed to create slip (student not found)" });
            }

            return Ok(slip);
        }

        [HttpGet("slips/{id}")]
        public async Task<IActionResult> GetSlip(int id)
        {
            var slip = await _registrationService.GetSlipByIdAsync(id);
            if (slip == null)
            {
                return NotFound();
            }

            return Ok(slip);
        }

        [HttpPost("slips/{id}/approve")]
        public async Task<IActionResult> ApproveSlip(int id, [FromBody] SlipApprovalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _registrationService.ApproveSlipAsync(id, userId, request.Comment);

            if (result)
            {
                return Ok(new { message = "Slip approved successfully" });
            }

            return BadRequest(new { message = "Failed to approve slip" });
        }

        [HttpPost("slips/{id}/reject")]
        public async Task<IActionResult> RejectSlip(int id, [FromBody] SlipApprovalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _registrationService.RejectSlipAsync(id, userId, request.Comment ?? "Rejected");

            if (result)
            {
                return Ok(new { message = "Slip rejected" });
            }

            return BadRequest(new { message = "Failed to reject slip" });
        }

        [HttpGet("students/{studentId}/history")]
        public async Task<IActionResult> GetStudentHistory(string studentId)
        {
            var slips = await _registrationService.GetStudentSlipsAsync(studentId);
            var gradeReports = await _context.GradeReports
                .Where(g => g.StudentID == studentId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                slips = slips,
                gradeReports = gradeReports
            });
        }
    }
}
