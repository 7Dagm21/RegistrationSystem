namespace AASTU.RegistrationSystem.API.DTOs
{
    public class SignUpRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty; // StudentID or StaffID
        public string UniversityEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class VerifyOTPRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }

    public class SignInRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
