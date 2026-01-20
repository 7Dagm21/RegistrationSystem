using MailKit.Net.Smtp;
using MimeKit;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOTPEmailAsync(string email, string name, string otp)
        {
            var subject = "AASTU Registration System - Email Verification OTP";
            var body = $@"
                <html>
                <body>
                    <h2>Email Verification</h2>
                    <p>Dear {name},</p>
                    <p>Your OTP for email verification is: <strong>{otp}</strong></p>
                    <p>This OTP will expire in 10 minutes.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Best regards,<br/>AASTU Registration System</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendVerificationLinkEmailAsync(string email, string name, string verificationUrl)
        {
            var subject = "AASTU Registration System - Email Verification";
            var body = $@"
                <html>
                <body>
                    <h2>Email Verification</h2>
                    <p>Dear {name},</p>
                    <p>Please verify your email by clicking the link below:</p>
                    <p><a href='{verificationUrl}'>Verify Email</a></p>
                    <p>This link will expire in 30 minutes.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Best regards,<br/>AASTU Registration System</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendNotificationEmailAsync(string email, string subject, string body)
        {
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string body, byte[] attachmentData, string attachmentFileName)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var senderEmail = emailSettings["SenderEmail"];
                var senderName = emailSettings["SenderName"];
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                // If email credentials are not configured, log and skip sending
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Email service not configured. Skipping email with attachment to {Email}", email);
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                // Add attachment
                bodyBuilder.Attachments.Add(attachmentFileName, attachmentData);

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, false);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Email with attachment sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with attachment to {Email}", email);
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var senderEmail = emailSettings["SenderEmail"];
                var senderName = emailSettings["SenderName"];
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                // If email credentials are not configured, log and skip sending
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Email service not configured. OTP: {OTP} for {Email}", body.Contains("OTP") ? ExtractOTP(body) : "N/A", toEmail);
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, false);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                // In development, we'll log the OTP instead of failing
                if (body.Contains("OTP"))
                {
                    var otp = ExtractOTP(body);
                    _logger.LogInformation("OTP for {Email}: {OTP}", toEmail, otp);
                }
            }
        }

        private string ExtractOTP(string body)
        {
            var startIndex = body.IndexOf("<strong>") + 8;
            var endIndex = body.IndexOf("</strong>", startIndex);
            if (startIndex > 7 && endIndex > startIndex)
            {
                return body.Substring(startIndex, endIndex - startIndex);
            }
            return "N/A";
        }
    }
}
