using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace TicketingSystem.Services
{
    public class QRCodeService : IQRCodeService
    {
        public string GenerateQRCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            var qrCodeBytes = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrCodeBytes);
        }

        public string GenerateTicketQRCode(string ticketNumber, int orderId, string memberName)
        {
            var qrData = $"TICKET:{ticketNumber}|ORDER:{orderId}|MEMBER:{memberName}|TIMESTAMP:{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            return GenerateQRCode(qrData);
        }

        public bool ValidateQRCode(string qrCodeData, string expectedTicketNumber)
        {
            try
            {
                var parts = qrCodeData.Split('|');
                if (parts.Length < 2) return false;

                var ticketPart = parts[0];
                var ticketNumber = ticketPart.Split(':')[1];

                return ticketNumber.Equals(expectedTicketNumber, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }

    public interface IQRCodeService
    {
        string GenerateQRCode(string data);
        string GenerateTicketQRCode(string ticketNumber, int orderId, string memberName);
        bool ValidateQRCode(string qrCodeData, string expectedTicketNumber);
    }
}
