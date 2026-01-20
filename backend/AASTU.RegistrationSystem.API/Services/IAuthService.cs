using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> SignUpAsync(SignUpRequest request, string ipAddress);
        Task<(bool Success, string Message)> VerifyOTPAsync(VerifyOTPRequest request);
        Task<AuthResponse?> SignInAsync(SignInRequest request, string ipAddress);
        string GenerateJwtToken(User user);
        Task<(bool Success, string Message)> VerifyEmailTokenAsync(string token);
    }
}
