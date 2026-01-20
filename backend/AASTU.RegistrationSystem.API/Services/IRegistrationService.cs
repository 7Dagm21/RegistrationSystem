using AASTU.RegistrationSystem.API.DTOs;

namespace AASTU.RegistrationSystem.API.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationSlipDto?> CreateSlipAsync(CreateSlipRequest request, string departmentHeadId);
        Task<RegistrationSlipDto?> CreateSlipByAdvisorAsync(CreateSlipRequest request, string advisorId);
        Task<List<RegistrationSlipDto>> GetStudentSlipsAsync(string studentId);
        Task<RegistrationSlipDto?> GetSlipByIdAsync(int slipId);
        Task<bool> ApproveSlipAsync(int slipId, string advisorId, string? comment);
        Task<bool> RejectSlipAsync(int slipId, string advisorId, string comment);
        Task<bool> VerifyCostSharingAsync(int slipId, string officerId);
        Task<bool> FinalizeRegistrationAsync(int slipId, string registrarId);
        Task<List<RegistrationSlipDto>> GetPendingApprovalsAsync(string role, string userId);
        Task<List<CourseDto>> GetCoursesByYearAsync(int academicYear, string department);
        Task<List<CourseDto>> GetCoursesAsync(int academicYear, string department, string? semester);
        Task<int> CalculateAcademicYear(string studentId);
    }
}
