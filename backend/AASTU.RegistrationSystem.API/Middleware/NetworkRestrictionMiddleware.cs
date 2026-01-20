using System.Net;

namespace AASTU.RegistrationSystem.API.Middleware
{
    public class NetworkRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NetworkRestrictionMiddleware> _logger;

        public NetworkRestrictionMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<NetworkRestrictionMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requireRestriction = _configuration.GetValue<bool>("NetworkSettings:RequireNetworkRestriction", true);

            if (requireRestriction)
            {
                var clientIp = GetClientIpAddress(context);
                var allowedRanges = _configuration.GetSection("NetworkSettings:AllowedIPRanges").Get<string[]>() ?? Array.Empty<string>();

                if (!IsIpAllowed(clientIp, allowedRanges))
                {
                    _logger.LogWarning("Access denied for IP: {IP}", clientIp);
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access denied. This system is only accessible within AASTU network.");
                    return;
                }
            }

            await _next(context);
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP (if behind proxy)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private bool IsIpAllowed(string ipAddress, string[] allowedRanges)
        {
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Unknown")
            {
                return false;
            }

            // In development, allow localhost
            if (ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress.StartsWith("::ffff:127.0.0.1"))
            {
                return true;
            }

            IPAddress? clientIp = null;
            if (!IPAddress.TryParse(ipAddress, out clientIp))
            {
                return false;
            }

            foreach (var range in allowedRanges)
            {
                if (IsIpInRange(clientIp, range))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsIpInRange(IPAddress ipAddress, string cidrRange)
        {
            try
            {
                var parts = cidrRange.Split('/');
                if (parts.Length != 2)
                {
                    return false;
                }

                if (!IPAddress.TryParse(parts[0], out IPAddress? networkAddress))
                {
                    return false;
                }

                if (!int.TryParse(parts[1], out int prefixLength))
                {
                    return false;
                }

                var networkBytes = networkAddress.GetAddressBytes();
                var ipBytes = ipAddress.GetAddressBytes();

                if (networkBytes.Length != ipBytes.Length)
                {
                    return false;
                }

                int bytesToCheck = prefixLength / 8;
                int bitsToCheck = prefixLength % 8;

                for (int i = 0; i < bytesToCheck; i++)
                {
                    if (networkBytes[i] != ipBytes[i])
                    {
                        return false;
                    }
                }

                if (bitsToCheck > 0 && bytesToCheck < networkBytes.Length)
                {
                    byte mask = (byte)(0xFF << (8 - bitsToCheck));
                    if ((networkBytes[bytesToCheck] & mask) != (ipBytes[bytesToCheck] & mask))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
