namespace AASTU.RegistrationSystem.API.Services
{
    public interface IQrCodeService
    {
        Task<string> GenerateQrCodeAsync(string serialNumber, string studentId, string semester);
    }
}
