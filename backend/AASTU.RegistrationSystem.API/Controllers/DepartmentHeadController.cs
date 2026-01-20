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
    [Authorize(Roles = "DepartmentHead")]
    public class DepartmentHeadController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ApplicationDbContext _context;

        public DepartmentHeadController(IRegistrationService registrationService, ApplicationDbContext context)
        {
            _registrationService = registrationService;
            _context = context;
        }

        [HttpGet("students/search")]
        public async Task<IActionResult> SearchStudent([FromQuery] string studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == studentId);
            if (student == null)
            {
                return NotFound();
            }

            int academicYear = await _registrationService.CalculateAcademicYear(studentId);

            return Ok(new
            {
                studentId = student.StudentID,
                fullName = student.FullName,
                email = student.UniversityEmail,
                department = student.Department,
                enrollmentYear = student.EnrollmentYear,
                academicYear = academicYear
            });
        }

        [HttpPost("slips/create")]
        public async Task<IActionResult> CreateSlip([FromBody] CreateSlipRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var slip = await _registrationService.CreateSlipAsync(request, userId);

            if (slip == null)
            {
                return BadRequest(new { message = "Failed to create slip" });
            }

            return Ok(slip);
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] int academicYear, [FromQuery] string department)
        {
            var courses = await _registrationService.GetCoursesByYearAsync(academicYear, department);
            return Ok(courses);
        }

        [HttpGet("curriculum")]
        public async Task<IActionResult> GetCurriculum([FromQuery] string department)
        {
            var courses = await _context.Courses
                .Where(c => c.Department == department)
                .OrderBy(c => c.AcademicYear)
                .ThenBy(c => c.CourseCode)
                .ToListAsync();

            var curriculum = courses.GroupBy(c => c.AcademicYear).Select(g => new
            {
                academicYear = g.Key,
                courses = g.Select(c => new
                {
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    c.Semester
                })
            });

            return Ok(curriculum);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDto courseDto, [FromQuery] int academicYear, [FromQuery] string department, [FromQuery] string? semester)
        {
            var course = new Models.Course
            {
                CourseCode = courseDto.CourseCode,
                CourseName = courseDto.CourseName,
                CreditHours = courseDto.CreditHours,
                AcademicYear = academicYear,
                Department = department,
                Semester = semester
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }
    }
}
