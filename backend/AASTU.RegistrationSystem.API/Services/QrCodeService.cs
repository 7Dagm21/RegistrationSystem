using QRCoder;
using System.Text.Json;

namespace AASTU.RegistrationSystem.API.Services
{
    public class QrCodeService : IQrCodeService
    {
        public async Task<string> GenerateQrCodeAsync(string serialNumber, string studentId, string semester)
        {
            var qrData = new
            {
                SerialNumber = serialNumber,
                StudentID = studentId,
                Semester = semester,
                VerifiedAt = DateTime.UtcNow
            };

            string jsonData = JsonSerializer.Serialize(qrData);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrDataObj = qrGenerator.CreateQrCode(jsonData, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrDataObj);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            return await Task.FromResult($"data:image/png;base64,{qrCodeImageAsBase64}");
        }
    }
}
