using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Data;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string role, string action, string? details, string? ipAddress)
        {
            var log = new AuditLog
            {
                UserID = userId,
                UserRole = role,
                Action = action,
                Details = details,
                IPAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLogDto>> GetLogsAsync(string? userId = null, string? role = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(l => l.UserID == userId);
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(l => l.UserRole == role);
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endDate.Value);
            }

            var logs = await query.OrderByDescending(l => l.Timestamp).Take(1000).ToListAsync();

            return logs.Select(l => new AuditLogDto
            {
                Id = l.Id,
                UserID = l.UserID,
                UserRole = l.UserRole,
                Action = l.Action,
                Details = l.Details,
                IPAddress = l.IPAddress,
                Timestamp = l.Timestamp
            }).ToList();
        }
    }
}
