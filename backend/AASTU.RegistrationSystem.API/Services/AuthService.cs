using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AASTU.RegistrationSystem.API.Data;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;
using BCrypt.Net;

namespace AASTU.RegistrationSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, IAuditLogService auditLogService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _auditLogService = auditLogService;
        }

        public async Task<(bool Success, string Message)> SignUpAsync(SignUpRequest request, string ipAddress)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.UniversityEmail || u.UserId == request.ID))
            {
                return (false, "User already exists");
            }

            // Validate email domain
            var allowedDomains = _configuration.GetSection("AllowedEmailDomains");
            var studentDomains = allowedDomains.GetSection("StudentDomains").Get<string[]>() ?? Array.Empty<string>();
            var staffDomains = allowedDomains.GetSection("StaffDomains").Get<string[]>() ?? Array.Empty<string>();

            bool isValidDomain = studentDomains.Any(d => request.UniversityEmail.EndsWith(d)) ||
                                staffDomains.Any(d => request.UniversityEmail.EndsWith(d));

            if (!isValidDomain)
            {
                return (false, "Invalid email domain. Only @aastustudent.edu.et and official staff emails are allowed.");
            }

            // Check if ID exists in base database
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == request.ID);
            Staff? staff = await _context.Staff.FirstOrDefaultAsync(s => s.StaffID == request.ID);

            if (student == null && staff == null)
            {
                return (false, "ID not found in system. Only pre-registered students and staff can sign up.");
            }

            // Verify email matches
            if (student != null && student.UniversityEmail != request.UniversityEmail)
            {
                return (false, "Email does not match student record");
            }

            if (staff != null && staff.Email != request.UniversityEmail)
            {
                return (false, "Email does not match staff record");
            }

            // Determine role
            string role = student?.Role ?? staff!.Role;

            // Create user account (no email verification required)
            var user = new User
            {
                FullName = request.FullName,
                UserId = request.ID,
                Email = request.UniversityEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                Department = student?.Department ?? staff!.Department,
                IsEmailVerified = true,
                EmailVerificationToken = null,
                EmailVerificationTokenExpiry = null,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(request.ID, role, "SignUp", $"User signed up: {request.UniversityEmail}", ipAddress);

            return (true, "Account created successfully. You can now sign in.");
        }

        public async Task<(bool Success, string Message)> VerifyOTPAsync(VerifyOTPRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return (false, "User not found");
            }

            if (user.IsEmailVerified)
            {
                return (false, "Email already verified");
            }

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return (false, "OTP expired. Please request a new one.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.OTP, user.EmailVerificationToken))
            {
                return (false, "Invalid OTP");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(user.UserId, user.Role, "EmailVerified", $"Email verified: {request.Email}", null);

            return (true, "Email verified successfully. You can now sign in.");
        }

        public async Task<AuthResponse?> SignInAsync(SignInRequest request, string ipAddress)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _auditLogService.LogAsync("", "Unknown", "LoginFailed", $"Failed login attempt: {request.Email}", ipAddress);
                return null;
            }

            if (!user.IsEmailVerified)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            await _auditLogService.LogAsync(user.UserId, user.Role, "Login", $"User logged in: {request.Email}", ipAddress);

            return new AuthResponse
            {
                Token = token,
                Role = user.Role,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"] ?? "1440")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

            public async Task<(bool Success, string Message)> VerifyEmailTokenAsync(string token)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
                if (user == null)
                {
                    return (false, "Invalid or expired verification link.");
                }
                if (user.IsEmailVerified)
                {
                    return (false, "Email already verified.");
                }
                if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
                {
                    return (false, "Verification link expired. Please request a new one.");
                }
                user.IsEmailVerified = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpiry = null;
                await _context.SaveChangesAsync();
                await _auditLogService.LogAsync(user.UserId, user.Role, "EmailVerified", $"Email verified via link: {user.Email}", null);
                return (true, "Email verified successfully. You can now sign in.");
            }
    }
}
