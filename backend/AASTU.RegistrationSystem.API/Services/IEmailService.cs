namespace AASTU.RegistrationSystem.API.Services
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string email, string name, string otp);
        Task SendNotificationEmailAsync(string email, string subject, string body);
        Task SendEmailWithAttachmentAsync(string email, string subject, string body, byte[] attachmentData, string attachmentFileName);
    }
}
