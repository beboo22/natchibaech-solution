using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TicketingSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #region face
        public async Task<bool> SendTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? "noreply@ticketing.com", "Ticketing System"),
                    Subject = $"Your Ticket - {ticketNumber}",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                var htmlBody = GenerateTicketEmailTemplate(ticketNumber, memberName,expiryDate, qrCodeBase64);
                mailMessage.Body = htmlBody;

                // Attach QR code as image
                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                var qrCodeAttachment = new Attachment(new MemoryStream(qrCodeBytes), $"ticket-{ticketNumber}.png", "image/png");
                mailMessage.Attachments.Add(qrCodeAttachment);

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket email to {Email}", email);
                return false;
            }

        }
        #endregion
        
        
        #region test by Twilio

        public async Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate)
        {
            try
            {
                // This is a placeholder for WhatsApp integration
                // You would integrate with WhatsApp Business API or a service like Twilio
                var whatsappSettings = _configuration.GetSection("WhatsAppSettings");
                var apiKey = whatsappSettings["ApiKey"];
                var apiUrl = whatsappSettings["ApiUrl"];

                var accountSid = whatsappSettings["AccountSid"];
                var authToken = whatsappSettings["AuthToken"];
                var fromNumber = whatsappSettings["FromNumber"];

                

                if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
                {
                    _logger.LogWarning("Twilio settings not configured");
                    return false;
                }

                TwilioClient.Init(accountSid, authToken);

                //// Create the message content
                ////var messageText = $"Hello {memberName},\n" +
                ////                  $"Your ticket number: {ticketNumber}\n" +
                ////                  $"Expiry date: {expiryDate:yyyy-MM-dd}\n" +
                ////                  $"Please present this ticket at entry.";

                var messageText = GenerateTicketWhatsAppMessage(ticketNumber, memberName, expiryDate);
                // Option 1: Send text only
                var message = await MessageResource.CreateAsync(
                    from: new Twilio.Types.PhoneNumber(fromNumber),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{phoneNumber}"),
                    body: messageText
                );
                //var mediaUrl = new Uri("https://yourdomain.com/qrcodes/" + ticketNumber + ".png");
                //var message = await MessageResource.CreateAsync(
                //    from: new Twilio.Types.PhoneNumber(fromNumber),
                //    to: new Twilio.Types.PhoneNumber($"whatsapp:{phoneNumber}"),
                //    body: messageText,
                //    mediaUrl: new List<Uri> { mediaUrl }
                //);

                // Placeholder for actual WhatsApp API call

                // You would implement the actual API call here
                _logger.LogInformation("WhatsApp message would be sent to {Phone}: {Message}", phoneNumber, message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket WhatsApp to {Phone}", phoneNumber);
                return false;
            }
        }

        #endregion


        //public async Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate)
        //{
        //    try
        //    {
        //        var whatsappSettings = _configuration.GetSection("WhatsAppSettings");
        //        var apiKey = whatsappSettings["ApiKey"];
        //        var apiUrl = whatsappSettings["ApiUrl"];

        //        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
        //        {
        //            _logger.LogWarning("WhatsApp settings not configured");
        //            return false;
        //        }

        //        // Format phone number correctly (must include country code, e.g., "201234567890")
        //        string formattedPhone = phoneNumber.StartsWith("+") ? phoneNumber.Substring(1) : phoneNumber;

        //        // Create the message body
        //        var messageBody = new
        //        {
        //            messaging_product = "whatsapp",
        //            to = formattedPhone,
        //            type = "template",
        //            template = new
        //            {
        //                name = "ticket_notification", // must be pre-approved template in WhatsApp Manager
        //                language = new { code = "en_US" },
        //                components = new object[]
        //                {
        //            new {
        //                type = "body",
        //                parameters = new object[]
        //                {
        //                    new { type = "text", text = memberName },
        //                    new { type = "text", text = ticketNumber },
        //                    new { type = "text", text = expiryDate.ToString("yyyy-MM-dd") }
        //                }
        //            },
        //            //new {
        //            //    type = "header",
        //            //    parameters = new object[]
        //            //    {
        //            //        new { type = "image", image = new { link = $"data:image/png;base64,{qrCodeBase64}" } }
        //            //    }
        //            //}
                        
                        
                        
        //                }
        //            }
        //        };

        //        using var client = new HttpClient();
        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        //        var json = System.Text.Json.JsonSerializer.Serialize(messageBody);
        //        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync(apiUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.LogInformation("WhatsApp ticket message sent successfully to {Phone}", phoneNumber);
        //            return true;
        //        }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            _logger.LogError("WhatsApp API error: {Error}", error);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to send ticket WhatsApp to {Phone}", phoneNumber);
        //        return false;
        //    }
        //}

        public async Task<bool> SendTicketBySMSAsync(string phoneNumber, string ticketNumber, string memberName, DateTime expiryDate)
        {
            try
            {
                // This is a placeholder for SMS integration
                // You would integrate with an SMS service like Twilio, AWS SNS, etc.
                var smsSettings = _configuration.GetSection("SmsSettings");
                var apiKey = smsSettings["ApiKey"];
                var apiUrl = smsSettings["ApiUrl"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
                {
                    _logger.LogWarning("SMS settings not configured");
                    return false;
                }

                var message = GenerateTicketSMSMessage(ticketNumber, memberName, expiryDate);

                // Placeholder for actual SMS API call
                _logger.LogInformation("SMS would be sent to {Phone}: {Message}", phoneNumber, message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket SMS to {Phone}", phoneNumber);
                return false;
            }
        }



        private static string GenerateTicketEmailTemplate(
    string ticketNumber,
    string memberName,
    DateTime expiryDate,
    string qrCodeBase64)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logo.png");
            var logoBytes = File.ReadAllBytes(logoPath);
            var logoBase64 = Convert.ToBase64String(logoBytes);




            return $@"
<!DOCTYPE html>
<html>
<head>
  <style>
    body {{
        font-family: Arial, sans-serif;
        margin: 0;
        padding: 20px;
        background-color: #f5f5f5;
    }}
    .container {{
        max-width: 900px;
        margin: 0 auto;
        background-color: white;
        padding: 20px;
        border-radius: 12px;
        box-shadow: 0 2px 12px rgba(0,0,0,0.1);
        display: flex;
        gap: 20px;
    }}
    .card {{
        flex: 1;
        border: 1px solid #eee;
        border-radius: 12px;
        padding: 20px;
        background: #fff;
    }}
    .header-logo {{
        text-align: center;
        margin-bottom: 15px;
    }}
    .header-logo img {{
        max-width: 120px;
    }}
    .ticket-info p {{
        margin: 6px 0;
        font-size: 14px;
    }}
    .qr-code, .barcode {{
        text-align: center;
        margin: 15px 0;
    }}
    .barcode img {{
        max-width: 200px;
    }}
    .wallet-buttons {{
        display: flex;
        justify-content: center;
        gap: 10px;
        margin-top: 10px;
    }}
    .wallet-buttons img {{
        height: 40px;
    }}
    .benefits ul {{
        list-style: none;
        padding: 0;
    }}
    .benefits li {{
        margin: 8px 0;
        font-size: 14px;
    }}
    .benefits li::before {{
        content: '✔️ ';
        color: green;
    }}
    .download-btn {{
        display: inline-block;
        background-color: #ff6600;
        color: #fff;
        padding: 10px 20px;
        border-radius: 8px;
        text-decoration: none;
        margin: 15px 0;
        font-weight: bold;
    }}
  </style>
</head>
<body>
  <div class='container'>

    <!-- LEFT CARD -->
    <div class='card'>
      <div class='header-logo'>
        <img src='https://drive.google.com/file/d/1fcEA27_rpnBE2bUrj-asSJMdDhwpIlRf/view?usp=sharing' alt='Logo' />
        <h3>NATCHI BAECH</h3>
      </div>

      <div class='ticket-info'>
        <p><strong>Member Name:</strong> {memberName}</p>
        <p><strong>ID Number:</strong> {ticketNumber}</p>
        <p><strong>Booking Date:</strong> {DateTime.UtcNow:dd/MM/yyyy}</p>
        <p><strong>Expiry Date:</strong> {expiryDate:dd/MM/yyyy}</p>
      </div>

      <div class='qr-code'>
        <img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />
      </div>

      
      <div class='wallet-buttons'>
        <img src='https://developer.apple.com/design/human-interface-guidelines/apple-pay/images/add-to-apple-wallet-button.png' alt='Apple Wallet' />
        <img src='https://upload.wikimedia.org/wikipedia/commons/5/5a/Google_Pay_Logo.svg' alt='Google Wallet' />
      </div>
    </div>

    <!-- RIGHT CARD -->
    <div class='card'>
      <h2 style='text-align:center;'>Digital Member Ticket</h2>
      <h3 style='text-align:center;'>NATCHI BAECH</h3>

      <div class='benefits'>
        <ul>
          <li>Visit VIP place</li>
          <li>Stay any long time</li>
          <li>Take any drink free</li>
          <li>Take any food free</li>
        </ul>
      </div>

      <div style='text-align:center;'>
        <a href='#' class='download-btn'>Download Ticket</a>
      </div>

      

  </div>
</body>
</html>";
        }

        private static string GenerateTicketWhatsAppMessage(string ticketNumber, string memberName, DateTime expiryDate)
        {
            return $@"🎫 *Your Event Ticket*

*Ticket Number:* {ticketNumber}
*Member Name:* {memberName}
*Valid Until:* {expiryDate:MMMM dd, yyyy}

Please present your QR code at the venue for entry.

⚠️ *Important:*
- Arrive 30 minutes early
- Ticket expires on {expiryDate:MMMM dd, yyyy}
- Non-transferable

Thank you for choosing our service! 🎉";
        }

        private static string GenerateTicketSMSMessage(string ticketNumber, string memberName, DateTime expiryDate)
        {
            return $"Your ticket {ticketNumber} for {memberName} is ready! Valid until {expiryDate:MMM dd, yyyy}. Check your email for QR code. Arrive 30min early.";
        }

        
    }

    public interface INotificationService
    {
        Task<bool> SendTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate);
        Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate);
        Task<bool> SendTicketBySMSAsync(string phoneNumber, string ticketNumber, string memberName, DateTime expiryDate);
        //Task<bool> SendEmail(string to, string subject, string body);

    }
}
