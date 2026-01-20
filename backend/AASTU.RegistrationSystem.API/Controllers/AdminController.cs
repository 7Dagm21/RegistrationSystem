using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AASTU.RegistrationSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Data;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public AdminController(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.UserId,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.IsActive,
                    u.IsEmailVerified,
                    u.LastLoginAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff()
        {
            var staff = await _context.Staff.ToListAsync();
            return Ok(staff);
        }

        [HttpPost("students")]
        public async Task<IActionResult> AddStudent([FromBody] Student student)
        {
            if (await _context.Students.AnyAsync(s => s.StudentID == student.StudentID))
            {
                return BadRequest(new { message = "Student already exists" });
            }

            if (await _context.Students.AnyAsync(s => s.UniversityEmail == student.UniversityEmail))
            {
                return BadRequest(new { message = "Email already registered" });
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync("SystemAdmin", "SystemAdmin", "StudentAdded", $"Added student: {student.StudentID} - {student.FullName}", null);

            return Ok(student);
        }

        [HttpPost("staff")]
        public async Task<IActionResult> AddStaff([FromBody] Staff staff)
        {
            if (await _context.Staff.AnyAsync(s => s.StaffID == staff.StaffID))
            {
                return BadRequest(new { message = "Staff already exists" });
            }

            if (await _context.Staff.AnyAsync(s => s.Email == staff.Email))
            {
                return BadRequest(new { message = "Email already registered" });
            }

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync("SystemAdmin", "SystemAdmin", "StaffAdded", $"Added staff: {staff.StaffID} - {staff.FullName}", null);

            return Ok(staff);
        }

        [HttpDelete("students/{studentId}")]
        public async Task<IActionResult> DeleteStudent(string studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == studentId);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync("SystemAdmin", "SystemAdmin", "StudentDeleted", $"Deleted student: {studentId}", null);

            return Ok(new { message = "Student deleted successfully" });
        }

        [HttpDelete("staff/{staffId}")]
        public async Task<IActionResult> DeleteStaff(string staffId)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.StaffID == staffId);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync("SystemAdmin", "SystemAdmin", "StaffDeleted", $"Deleted staff: {staffId}", null);

            return Ok(new { message = "Staff deleted successfully" });
        }

        [HttpPut("users/{id}/reset")]
        public async Task<IActionResult> ResetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Reset password to default (in production, send reset email)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("DefaultPassword123!");
            user.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User reset successfully" });
        }

        [HttpPut("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deactivated" });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] string? userId, [FromQuery] string? role, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var logs = await _auditLogService.GetLogsAsync(userId, role, startDate, endDate);
            return Ok(logs);
        }
    }
}
