using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateApprovedSlipPdfAsync(RegistrationSlipDto slip);
        Task<byte[]> GenerateCostSharingFormPdfAsync(CostSharingForm form, Student student, RegistrationSlip slip);
        Task<byte[]> GenerateGradeReportPdfAsync(GradeReport gradeReport);
    }
}
