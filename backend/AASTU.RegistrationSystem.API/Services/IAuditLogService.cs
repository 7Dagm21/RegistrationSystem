namespace AASTU.RegistrationSystem.API.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string role, string action, string? details, string? ipAddress);
        Task<List<AuditLogDto>> GetLogsAsync(string? userId = null, string? role = null, DateTime? startDate = null, DateTime? endDate = null);
    }

    public class AuditLogDto
    {
        public int Id { get; set; }
        public string UserID { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
