using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;
using AASTU.RegistrationSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Data;

namespace AASTU.RegistrationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GradeReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly IAuditLogService _auditLogService;

        public GradeReportController(ApplicationDbContext context, IPdfService pdfService, IAuditLogService auditLogService)
        {
            _context = context;
            _pdfService = pdfService;
            _auditLogService = auditLogService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Registrar")]
        public async Task<IActionResult> CreateGradeReport([FromBody] CreateGradeReportRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrWhiteSpace(request.StudentID))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                if (request.Courses == null || request.Courses.Count == 0)
                {
                    return BadRequest(new { message = "At least one course is required" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var registrar = await _context.Staff.FirstOrDefaultAsync(s => s.StaffID == userId);
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == request.StudentID);

                if (student == null)
                {
                    return NotFound(new { message = $"Student with ID '{request.StudentID}' not found" });
                }


                // Calculate GradePoint and LetterGrade for each course based on NumberGrade
                foreach (var course in request.Courses)
                {
                    // Ethiopian standard grade mapping (example, adjust as needed)
                    if (course.NumberGrade >= 90)
                    {
                        course.LetterGrade = "A+";
                        course.GradePoint = 4.0m * course.Credit;
                    }
                    else if (course.NumberGrade >= 85)
                    {
                        course.LetterGrade = "A";
                        course.GradePoint = 4.0m * course.Credit;
                    }
                    else if (course.NumberGrade >= 80)
                    {
                        course.LetterGrade = "A-";
                        course.GradePoint = 3.75m * course.Credit;
                    }
                    else if (course.NumberGrade >= 75)
                    {
                        course.LetterGrade = "B+";
                        course.GradePoint = 3.5m * course.Credit;
                    }
                    else if (course.NumberGrade >= 70)
                    {
                        course.LetterGrade = "B";
                        course.GradePoint = 3.0m * course.Credit;
                    }
                    else if (course.NumberGrade >= 65)
                    {
                        course.LetterGrade = "B-";
                        course.GradePoint = 2.75m * course.Credit;
                    }
                    else if (course.NumberGrade >= 60)
                    {
                        course.LetterGrade = "C+";
                        course.GradePoint = 2.5m * course.Credit;
                    }
                    else if (course.NumberGrade >= 50)
                    {
                        course.LetterGrade = "C";
                        course.GradePoint = 2.0m * course.Credit;
                    }
                    else if (course.NumberGrade >= 45)
                    {
                        course.LetterGrade = "D";
                        course.GradePoint = 1.0m * course.Credit;
                    }
                    else
                    {
                        course.LetterGrade = "F";
                        course.GradePoint = 0.0m;
                    }
                }

                // Calculate semester totals using backend-calculated GradePoint
                decimal semesterCredit = request.Courses.Sum(c => c.Credit);
                decimal semesterGP = request.Courses.Sum(c => c.GradePoint); // Sum(Credit × GradePointValue)
                decimal semesterANG = semesterCredit > 0 ? semesterGP / semesterCredit : 0; // GPA

                // Calculate cumulative totals
                decimal cumulativeCredit = (request.PreviousCredit ?? 0) + semesterCredit;
                decimal cumulativeGP = (request.PreviousGP ?? 0) + semesterGP;
                decimal cumulativeANG = cumulativeCredit > 0 ? cumulativeGP / cumulativeCredit : 0; // CGPA

                var gradeReport = new GradeReport
                {
                    StudentID = request.StudentID,
                    StudentName = student.FullName,
                    Major = request.Major ?? student.Department,
                    AdmissionClassification = request.AdmissionClassification ?? "Regular",
                    Program = request.Program ?? "Degree",
                    Year = request.Year,
                    Semester = request.Semester,
                    AcademicYear = request.AcademicYear,
                    GradesJson = JsonSerializer.Serialize(request.Courses),
                    PreviousCredit = request.PreviousCredit,
                    PreviousGP = request.PreviousGP,
                    PreviousANG = request.PreviousANG,
                    SemesterCredit = semesterCredit,
                    SemesterGP = semesterGP,
                    SemesterANG = semesterANG,
                    CumulativeCredit = cumulativeCredit,
                    CumulativeGP = cumulativeGP,
                    CumulativeANG = cumulativeANG,
                    GPA = semesterANG,
                    CGPA = cumulativeANG,
                    Remark = request.Remark ?? "X Promoted",
                    RegistrarRecorderName = request.RegistrarRecorderName ?? registrar?.FullName,
                    RegistrarSignedDate = request.RegistrarSignedDate ?? DateTime.UtcNow,
                    GeneratedBy = registrar?.FullName ?? userId,
                    GeneratedAt = DateTime.UtcNow,
                    Status = "Created"
                };

                _context.GradeReports.Add(gradeReport);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(userId, "Registrar", "GradeReportCreated", $"Created grade report for student {request.StudentID}", null);

                return Ok(new 
                { 
                    message = "Grade report created successfully", 
                    id = gradeReport.Id,
                    gpa = semesterANG,
                    cgpa = cumulativeANG
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the grade report", error = ex.Message });
            }
        }

        [HttpGet("pending-approvals")]
        [Authorize(Roles = "DepartmentHead")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var gradeReports = await _context.GradeReports
                .Where(g => g.Status == "Created")
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var result = gradeReports.Select(g => new GradeReportDto
            {
                Id = g.Id,
                StudentID = g.StudentID,
                StudentName = g.StudentName,
                Major = g.Major,
                Semester = g.Semester,
                AcademicYear = g.AcademicYear,
                Year = g.Year,
                Status = g.Status,
                CreatedAt = g.CreatedAt
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "DepartmentHead,Registrar")]
        public async Task<IActionResult> GetGradeReport(int id)
        {
            var gradeReport = await _context.GradeReports.FirstOrDefaultAsync(g => g.Id == id);
            if (gradeReport == null)
            {
                return NotFound();
            }

            var courses = JsonSerializer.Deserialize<List<CourseGradeDto>>(gradeReport.GradesJson) ?? new List<CourseGradeDto>();

            var dto = new GradeReportDto
            {
                Id = gradeReport.Id,
                StudentID = gradeReport.StudentID,
                StudentName = gradeReport.StudentName,
                Major = gradeReport.Major,
                AdmissionClassification = gradeReport.AdmissionClassification,
                Program = gradeReport.Program,
                Year = gradeReport.Year,
                Semester = gradeReport.Semester,
                AcademicYear = gradeReport.AcademicYear,
                Courses = courses,
                PreviousCredit = gradeReport.PreviousCredit,
                PreviousGP = gradeReport.PreviousGP,
                PreviousANG = gradeReport.PreviousANG,
                SemesterCredit = gradeReport.SemesterCredit,
                SemesterGP = gradeReport.SemesterGP,
                SemesterANG = gradeReport.SemesterANG,
                CumulativeCredit = gradeReport.CumulativeCredit,
                CumulativeGP = gradeReport.CumulativeGP,
                CumulativeANG = gradeReport.CumulativeANG,
                GPA = gradeReport.GPA,
                CGPA = gradeReport.CGPA,
                Remark = gradeReport.Remark,
                RegistrarRecorderName = gradeReport.RegistrarRecorderName,
                RegistrarSignedDate = gradeReport.RegistrarSignedDate,
                GeneratedBy = gradeReport.GeneratedBy,
                GeneratedAt = gradeReport.GeneratedAt,
                Status = gradeReport.Status,
                ApprovedBy = gradeReport.ApprovedBy,
                ApprovedAt = gradeReport.ApprovedAt,
                CreatedAt = gradeReport.CreatedAt
            };

            return Ok(dto);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "DepartmentHead")]
        public async Task<IActionResult> ApproveGradeReport(int id, [FromBody] GradeReportApprovalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gradeReport = await _context.GradeReports.FirstOrDefaultAsync(g => g.Id == id);

            if (gradeReport == null)
            {
                return NotFound();
            }

            if (request.IsApproved)
            {
                gradeReport.Status = "DepartmentHeadApproved";
                gradeReport.ApprovedBy = userId;
                gradeReport.ApprovedAt = DateTime.UtcNow;
            }
            else
            {
                gradeReport.Status = "Rejected";
                gradeReport.RejectionReason = request.Comment;
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(userId, "DepartmentHead", request.IsApproved ? "GradeReportApproved" : "GradeReportRejected", 
                $"Grade report {id} {(request.IsApproved ? "approved" : "rejected")} for student {gradeReport.StudentID}", null);

            return Ok(new { message = $"Grade report {(request.IsApproved ? "approved" : "rejected")} successfully" });
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentGradeReports(string studentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != studentId)
            {
                return Forbid();
            }

            var gradeReports = await _context.GradeReports
                .Where(g => g.StudentID == studentId && g.Status == "DepartmentHeadApproved")
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var result = gradeReports.Select(g =>
            {
                var courses = JsonSerializer.Deserialize<List<CourseGradeDto>>(g.GradesJson) ?? new List<CourseGradeDto>();
                return new GradeReportDto
                {
                    Id = g.Id,
                    StudentID = g.StudentID,
                    StudentName = g.StudentName,
                    Major = g.Major,
                    AdmissionClassification = g.AdmissionClassification,
                    Program = g.Program,
                    Year = g.Year,
                    Semester = g.Semester,
                    AcademicYear = g.AcademicYear,
                    Courses = courses,
                    PreviousCredit = g.PreviousCredit,
                    PreviousGP = g.PreviousGP,
                    PreviousANG = g.PreviousANG,
                    SemesterCredit = g.SemesterCredit,
                    SemesterGP = g.SemesterGP,
                    SemesterANG = g.SemesterANG,
                    CumulativeCredit = g.CumulativeCredit,
                    CumulativeGP = g.CumulativeGP,
                    CumulativeANG = g.CumulativeANG,
                    GPA = g.GPA,
                    CGPA = g.CGPA,
                    Remark = g.Remark,
                    RegistrarRecorderName = g.RegistrarRecorderName,
                    RegistrarSignedDate = g.RegistrarSignedDate,
                    GeneratedBy = g.GeneratedBy,
                    GeneratedAt = g.GeneratedAt,
                    Status = g.Status,
                    CreatedAt = g.CreatedAt
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}/pdf")]
        [Authorize(Roles = "Student,Registrar,DepartmentHead")]
        public async Task<IActionResult> DownloadGradeReportPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var gradeReport = await _context.GradeReports.FirstOrDefaultAsync(g => g.Id == id);

            if (gradeReport == null)
            {
                return NotFound();
            }

            // Students can only download approved reports
            if (userRole == "Student" && gradeReport.Status != "DepartmentHeadApproved")
            {
                return Forbid();
            }

            // Students can only download their own reports
            if (userRole == "Student" && gradeReport.StudentID != userId)
            {
                return Forbid();
            }

            var pdfBytes = await _pdfService.GenerateGradeReportPdfAsync(gradeReport);
            return File(pdfBytes, "application/pdf", $"GradeReport_{gradeReport.StudentID}_{gradeReport.Semester}_{gradeReport.AcademicYear}.pdf");
        }
    }
}
