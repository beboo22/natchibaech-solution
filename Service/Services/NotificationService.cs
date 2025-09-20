using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json.Linq;

namespace TicketingSystem.Services
{
    #region old

    //    public class NotificationService : INotificationService
    //    {
    //        private readonly IConfiguration _configuration;
    //        private readonly ILogger<NotificationService> _logger;

    //        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
    //        {
    //            _configuration = configuration;
    //            _logger = logger;
    //        }






    //        #region face
    //        public async Task<bool> SendSingleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone)
    //        {
    //            try
    //            {
    //                var smtpSettings = _configuration.GetSection("SmtpSettings");
    //                var smtpHost = smtpSettings["Host"];
    //                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
    //                var smtpUsername = smtpSettings["Username"];
    //                var smtpPassword = smtpSettings["Password"];
    //                var fromEmail = smtpSettings["FromEmail"];

    //                using var client = new SmtpClient(smtpHost, smtpPort)
    //                {
    //                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
    //                    EnableSsl = true
    //                };

    //                var mailMessage = new MailMessage
    //                {
    //                    From = new MailAddress(fromEmail ?? "noreply@ticketing.com", "Ticketing System"),
    //                    Subject = $"Your Ticket - {ticketNumber}",
    //                    IsBodyHtml = true
    //                };

    //                mailMessage.To.Add(email);

    //                var htmlBody = GenerateTicketEmailTemplateWithoutPartner(ticketNumber, memberName, email, phone, bookingDate, expiryDate, qrCodeBase64);
    //                mailMessage.Body = htmlBody;

    //                // Attach QR code as image
    //                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
    //                var qrCodeAttachment = new Attachment(new MemoryStream(qrCodeBytes), $"ticket-{ticketNumber}.png", "image/png");
    //                mailMessage.Attachments.Add(qrCodeAttachment);

    //                await client.SendMailAsync(mailMessage);
    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send ticket email to {Email}", email);
    //                return false;
    //            }

    //        }
    //        public async Task<bool> SendCoupleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string partnerName)
    //        {
    //            try
    //            {
    //                var smtpSettings = _configuration.GetSection("SmtpSettings");
    //                var smtpHost = smtpSettings["Host"];
    //                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
    //                var smtpUsername = smtpSettings["Username"];
    //                var smtpPassword = smtpSettings["Password"];
    //                var fromEmail = smtpSettings["FromEmail"];

    //                using var client = new SmtpClient(smtpHost, smtpPort)
    //                {
    //                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
    //                    EnableSsl = true
    //                };

    //                var mailMessage = new MailMessage
    //                {
    //                    From = new MailAddress(fromEmail ?? "noreply@ticketing.com", "Ticketing System"),
    //                    Subject = $"Your Ticket - {ticketNumber}",
    //                    IsBodyHtml = true
    //                };

    //                mailMessage.To.Add(email);

    //                var htmlBody = GenerateTicketEmailTemplateWithPartner(ticketNumber, memberName, email, phone, partnerName, bookingDate, expiryDate, qrCodeBase64);
    //                mailMessage.Body = htmlBody;

    //                // Attach QR code as image
    //                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
    //                var qrCodeAttachment = new Attachment(new MemoryStream(qrCodeBytes), $"ticket-{ticketNumber}.png", "image/png");
    //                mailMessage.Attachments.Add(qrCodeAttachment);

    //                await client.SendMailAsync(mailMessage);
    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send ticket email to {Email}", email);
    //                return false;
    //            }

    //        }
    //        #endregion


    //        #region test by Twilio

    //        public async Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate)
    //        {
    //            try
    //            {
    //                // This is a placeholder for WhatsApp integration
    //                // You would integrate with WhatsApp Business API or a service like Twilio
    //                var whatsappSettings = _configuration.GetSection("WhatsAppSettings");
    //                var apiKey = whatsappSettings["ApiKey"];
    //                var apiUrl = whatsappSettings["ApiUrl"];

    //                var accountSid = whatsappSettings["AccountSid"];
    //                var authToken = whatsappSettings["AuthToken"];
    //                var fromNumber = whatsappSettings["FromNumber"];



    //                if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
    //                {
    //                    _logger.LogWarning("Twilio settings not configured");
    //                    return false;
    //                }

    //                TwilioClient.Init(accountSid, authToken);

    //                //// Create the message content
    //                ////var messageText = $"Hello {memberName},\n" +
    //                ////                  $"Your ticket number: {ticketNumber}\n" +
    //                ////                  $"Expiry date: {expiryDate:yyyy-MM-dd}\n" +
    //                ////                  $"Please present this ticket at entry.";

    //                var messageText = GenerateTicketWhatsAppMessage(ticketNumber, memberName, expiryDate);
    //                // Option 1: Send text only
    //                var message = await MessageResource.CreateAsync(
    //                    from: new Twilio.Types.PhoneNumber(fromNumber),
    //                    to: new Twilio.Types.PhoneNumber($"whatsapp:{phoneNumber}"),
    //                    body: messageText
    //                );
    //                //var mediaUrl = new Uri("https://yourdomain.com/qrcodes/" + ticketNumber + ".png");
    //                //var message = await MessageResource.CreateAsync(
    //                //    from: new Twilio.Types.PhoneNumber(fromNumber),
    //                //    to: new Twilio.Types.PhoneNumber($"whatsapp:{phoneNumber}"),
    //                //    body: messageText,
    //                //    mediaUrl: new List<Uri> { mediaUrl }
    //                //);

    //                // Placeholder for actual WhatsApp API call

    //                // You would implement the actual API call here
    //                _logger.LogInformation("WhatsApp message would be sent to {Phone}: {Message}", phoneNumber, message);

    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send ticket WhatsApp to {Phone}", phoneNumber);
    //                return false;
    //            }
    //        }

    //        #endregion


    //        //public async Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate)
    //        //{
    //        //    try
    //        //    {
    //        //        var whatsappSettings = _configuration.GetSection("WhatsAppSettings");
    //        //        var apiKey = whatsappSettings["ApiKey"];
    //        //        var apiUrl = whatsappSettings["ApiUrl"];

    //        //        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
    //        //        {
    //        //            _logger.LogWarning("WhatsApp settings not configured");
    //        //            return false;
    //        //        }

    //        //        // Format phone number correctly (must include country code, e.g., "201234567890")
    //        //        string formattedPhone = phoneNumber.StartsWith("+") ? phoneNumber.Substring(1) : phoneNumber;

    //        //        // Create the message body
    //        //        var messageBody = new
    //        //        {
    //        //            messaging_product = "whatsapp",
    //        //            to = formattedPhone,
    //        //            type = "template",
    //        //            template = new
    //        //            {
    //        //                name = "ticket_notification", // must be pre-approved template in WhatsApp Manager
    //        //                language = new { code = "en_US" },
    //        //                components = new object[]
    //        //                {
    //        //            new {
    //        //                type = "body",
    //        //                parameters = new object[]
    //        //                {
    //        //                    new { type = "text", text = memberName },
    //        //                    new { type = "text", text = ticketNumber },
    //        //                    new { type = "text", text = expiryDate.ToString("yyyy-MM-dd") }
    //        //                }
    //        //            },
    //        //            //new {
    //        //            //    type = "header",
    //        //            //    parameters = new object[]
    //        //            //    {
    //        //            //        new { type = "image", image = new { link = $"data:image/png;base64,{qrCodeBase64}" } }
    //        //            //    }
    //        //            //}



    //        //                }
    //        //            }
    //        //        };

    //        //        using var client = new HttpClient();
    //        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

    //        //        var json = System.Text.Json.JsonSerializer.Serialize(messageBody);
    //        //        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    //        //        var response = await client.PostAsync(apiUrl, content);

    //        //        if (response.IsSuccessStatusCode)
    //        //        {
    //        //            _logger.LogInformation("WhatsApp ticket message sent successfully to {Phone}", phoneNumber);
    //        //            return true;
    //        //        }
    //        //        else
    //        //        {
    //        //            var error = await response.Content.ReadAsStringAsync();
    //        //            _logger.LogError("WhatsApp API error: {Error}", error);
    //        //            return false;
    //        //        }
    //        //    }
    //        //    catch (Exception ex)
    //        //    {
    //        //        _logger.LogError(ex, "Failed to send ticket WhatsApp to {Phone}", phoneNumber);
    //        //        return false;
    //        //    }
    //        //}

    //        public async Task<bool> SendTicketBySMSAsync(string phoneNumber, string ticketNumber, string memberName, DateTime expiryDate)
    //        {
    //            try
    //            {
    //                // This is a placeholder for SMS integration
    //                // You would integrate with an SMS service like Twilio, AWS SNS, etc.
    //                var smsSettings = _configuration.GetSection("SmsSettings");
    //                var apiKey = smsSettings["ApiKey"];
    //                var apiUrl = smsSettings["ApiUrl"];

    //                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
    //                {
    //                    _logger.LogWarning("SMS settings not configured");
    //                    return false;
    //                }

    //                var message = GenerateTicketSMSMessage(ticketNumber, memberName, expiryDate);

    //                // Placeholder for actual SMS API call
    //                _logger.LogInformation("SMS would be sent to {Phone}: {Message}", phoneNumber, message);

    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send ticket SMS to {Phone}", phoneNumber);
    //                return false;
    //            }
    //        }



    //        // ===== METHOD 1: With Partner Name =====
    //        private static string GenerateTicketEmailTemplateWithPartner(
    //            string ticketNumber,
    //            string memberName,
    //            string email,
    //            string phone,
    //            string partnerName,
    //            DateTime bookingDate,
    //            DateTime expiryDate,
    //            string qrCodeBase64)
    //        {
    //            return $@"
    //<!DOCTYPE html>
    //<html>
    //<head>
    //  <style>
    //    body {{
    //        font-family: Arial, sans-serif;
    //        margin: 0;
    //        padding: 20px;
    //        background-color: #f5f5f5;
    //    }}
    //    .container {{
    //        max-width: 950px;
    //        margin: 0 auto;
    //        background-color: #fff;
    //        border-radius: 12px;
    //        box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    //        display: flex;
    //        gap: 20px;
    //        padding: 20px;
    //    }}
    //    .card {{
    //        flex: 1;
    //        border-radius: 12px;
    //        padding: 20px;
    //        background: #fff;
    //    }}
    //    .left-header {{
    //        height: 100px;
    //        background: linear-gradient(90deg, #ff6600, #ff8c1a);
    //        border-radius: 12px 12px 0 0;
    //        display: flex;
    //        align-items: center;
    //        justify-content: center;
    //    }}
    //    .info {{
    //        margin-top: 15px;
    //        font-size: 14px;
    //    }}
    //    .info p {{
    //        margin: 6px 0;
    //    }}
    //    .qr-code {{
    //        margin-top: 20px;
    //        text-align: center;
    //    }}
    //    .benefits ul {{
    //        list-style: none;
    //        padding: 0;
    //    }}
    //    .benefits li {{
    //        margin: 8px 0;
    //        font-size: 14px;
    //    }}
    //    .download-btn {{
    //        display: inline-block;
    //        background-color: #ff6600;
    //        color: #fff;
    //        padding: 12px 20px;
    //        border-radius: 8px;
    //        text-decoration: none;
    //        margin: 15px 0;
    //        font-weight: bold;
    //    }}
    //    .wallet-buttons {{
    //        display: flex;
    //        justify-content: center;
    //        gap: 10px;
    //        margin-top: 15px;
    //    }}
    //    .wallet-buttons img {{
    //        height: 40px;
    //    }}
    //  </style>
    //</head>
    //<body>
    //  <div class='container'>

    //    <!-- LEFT CARD -->
    //    <div class='card'>
    //      <div class='left-header'>
    //        <img src='https://images2.imgbox.com/59/b4/L3jpwh9t_o.png' alt='Logo' width='80'/>
    //      </div>
    //      <div class='info'>
    //        <p><strong>Member name:</strong> {memberName}</p>
    //        <p><strong>Email:</strong> {email}</p>
    //        <p><strong>Phone:</strong> {phone}</p>
    //        <p><strong>Partner name:</strong> {partnerName}</p>
    //        <p><strong>ID number:</strong> {ticketNumber}</p>
    //        <p><strong>Booking day:</strong> {bookingDate:dd/MM/yyyy}</p>
    //        <p><strong>Expiry Booking:</strong> {expiryDate:dd/MM/yyyy}</p>
    //      </div>
    //      <div class='qr-code'>
    //        <img src='data:image/png;base64,{qrCodeBase64}' width='120' alt='QR Code'/>
    //      </div>
    //      <div class='wallet-buttons'>
    //        <img src='https://images2.imgbox.com/ab/16/KY1j2muj_o.jpeg' alt='Apple Wallet'/>
    //        <img src='https://images2.imgbox.com/5e/02/EbTMPGEm_o.png' alt='Google Wallet'/>
    //      </div>
    //    </div>

    //    <!-- RIGHT CARD -->
    //    <div class='card'>
    //      <h2 style='text-align:center;'>Digital member card</h2>
    //      <p style='text-align:center;'>Membership Benefits:</p>
    //      <p style='text-align:center;'>Unlimited access to the resort throughout the year</p>

    //      <div class='benefits'>
    //        <ul>
    //          <li>Visit vip place</li>
    //          <li>Exclusive discounts on food and beverages</li>
    //          <li>Priority booking for members-only Natchi events</li>
    //          <li>VIP seating reservations at the beach and club areas</li>
    //        </ul>
    //      </div>

    //      <div style='text-align:center;'>
    //        <a href='#' class='download-btn'>Download membership card</a>
    //      </div>
    //    </div>

    //  </div>
    //</body>
    //</html>";
    //        }

    //        // ===== METHOD 2: Without Partner Name =====
    //        private static string GenerateTicketEmailTemplateWithoutPartner(
    //            string ticketNumber,
    //            string memberName,
    //            string email,
    //            string phone,
    //            DateTime bookingDate,
    //            DateTime expiryDate,
    //            string qrCodeBase64)
    //        {
    //            return $@"
    //<!DOCTYPE html>
    //<html>
    //<head>
    //  <style>
    //    body {{
    //        font-family: Arial, sans-serif;
    //        margin: 0;
    //        padding: 20px;
    //        background-color: #f5f5f5;
    //    }}
    //    .container {{
    //        max-width: 950px;
    //        margin: 0 auto;
    //        background-color: #fff;
    //        border-radius: 12px;
    //        box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    //        display: flex;
    //        gap: 20px;
    //        padding: 20px;
    //    }}
    //    .card {{
    //        flex: 1;
    //        border-radius: 12px;
    //        padding: 20px;
    //        background: #fff;
    //    }}
    //    .left-header {{
    //        height: 100px;
    //        background: linear-gradient(90deg, #ff6600, #ff8c1a);
    //        border-radius: 12px 12px 0 0;
    //        display: flex;
    //        align-items: center;
    //        justify-content: center;
    //    }}
    //    .info {{
    //        margin-top: 15px;
    //        font-size: 14px;
    //    }}
    //    .info p {{
    //        margin: 6px 0;
    //    }}
    //    .qr-code {{
    //        margin-top: 20px;
    //        text-align: center;
    //    }}
    //    .benefits ul {{
    //        padding: 0;
    //    }}
    //    .benefits li {{
    //        margin: 8px 0;
    //        font-size: 14px;
    //    }}
    //    .download-btn {{
    //        display: inline-block;
    //        background-color: #ff6600;
    //        color: #fff;
    //        padding: 12px 20px;
    //        border-radius: 8px;
    //        text-decoration: none;
    //        margin: 15px 0;
    //        font-weight: bold;
    //    }}
    //    .wallet-buttons {{
    //        display: flex;
    //        justify-content: center;
    //        gap: 10px;
    //        margin-top: 15px;
    //    }}
    //    .wallet-buttons img {{
    //        height: 40px;
    //    }}
    //  </style>
    //</head>
    //<body>
    //  <div class='container'>

    //    <!-- LEFT CARD -->
    //    <div class='card'>
    //      <div class='left-header'>
    //        <img src='https://images2.imgbox.com/59/b4/L3jpwh9t_o.png' alt='Logo' width='80'/>
    //      </div>
    //      <div class='info'>
    //        <p><strong>Member name:</strong> {memberName}</p>
    //        <p><strong>Email:</strong> {email}</p>
    //        <p><strong>Phone:</strong> {phone}</p>
    //        <p><strong>ID number:</strong> {ticketNumber}</p>
    //        <p><strong>Booking day:</strong> {bookingDate:dd/MM/yyyy}</p>
    //        <p><strong>Expiry Booking:</strong> {expiryDate:dd/MM/yyyy}</p>
    //      </div>
    //      <div class='qr-code'>
    //        <img src='data:image/png;base64,{qrCodeBase64}' width='120' alt='QR Code'/>
    //      </div>
    //      <div class='wallet-buttons'>
    //        <img src='https://images2.imgbox.com/ab/16/KY1j2muj_o.jpeg' alt='Apple Wallet'/>
    //        <img src='https://images2.imgbox.com/5e/02/EbTMPGEm_o.png' alt='Google Wallet'/>
    //      </div>
    //    </div>

    //    <!-- RIGHT CARD -->
    //    <div class='card'>
    //      <h2 style='text-align:center;'>Digital member card</h2>
    //      <p style='text-align:center;'>Membership Benefits:</p>
    //      <p style='text-align:center;'>Unlimited access to the resort throughout the year</p>

    //      <div class='benefits'>
    //        <ul>
    //          <li>Visit vip place</li>
    //          <li>Exclusive discounts on food and beverages</li>
    //          <li>Priority booking for members-only Natchi events</li>
    //          <li>VIP seating reservations at the beach and club areas</li>
    //        </ul>
    //      </div>
    //    </div>

    //  </div>
    //</body>
    //</html>";
    //        }


    //        private static string GenerateTicketWhatsAppMessage(string ticketNumber, string memberName, DateTime expiryDate)
    //        {
    //            return $@"🎫 *Your Event Ticket*

    //*Ticket Number:* {ticketNumber}
    //*Member Name:* {memberName}
    //*Valid Until:* {expiryDate:MMMM dd, yyyy}

    //Please present your QR code at the venue for entry.

    //⚠️ *Important:*
    //- Arrive 30 minutes early
    //- Ticket expires on {expiryDate:MMMM dd, yyyy}
    //- Non-transferable

    //Thank you for choosing our service! 🎉";
    //        }

    //        private static string GenerateTicketSMSMessage(string ticketNumber, string memberName, DateTime expiryDate)
    //        {
    //            return $"Your ticket {ticketNumber} for {memberName} is ready! Valid until {expiryDate:MMM dd, yyyy}. Check your email for QR code. Arrive 30min early.";
    //        }
    //        public async Task<bool> SendRejectedEmail(string email, string memberName)
    //        {
    //            try
    //            {
    //                var smtpSettings = _configuration.GetSection("SmtpSettings");
    //                var smtpHost = smtpSettings["Host"];
    //                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
    //                var smtpUsername = smtpSettings["Username"];
    //                var smtpPassword = smtpSettings["Password"];
    //                var fromEmail = smtpSettings["FromEmail"];

    //                using var client = new SmtpClient(smtpHost, smtpPort)
    //                {
    //                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
    //                    EnableSsl = true
    //                };

    //                using var mailMessage = new MailMessage
    //                {
    //                    From = new MailAddress(fromEmail, "Natchi Beach"),
    //                    Subject = "Membership Application Update - Natchi Beach",
    //                    IsBodyHtml = true,
    //                    Body = $@"
    //<html lang='en'>
    //<body style='font-family: Arial, sans-serif; font-size:14px; line-height:1.8;'>
    //    <p>Dear {memberName},</p>
    //    <p>
    //        Thank you for your interest in joining the Natchi Beach membership.
    //    </p>
    //    <p>
    //        We apologize, but we would like to inform you that the number of members has currently reached its limit.
    //    </p>
    //    <p>
    //        Thank you for your understanding and interest.
    //    </p>
    //    <p>
    //        Best regards, <br/>
    //        Natchi Beach Team
    //    </p>
    //</body>
    //</html>"
    //                };

    //                mailMessage.To.Add(email);

    //                await client.SendMailAsync(mailMessage);
    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send rejection email to {Email}", email);
    //                return false;
    //            }
    //        }

    //        public async Task<bool> SendApprovedEmail(string email, string memberName)
    //        {
    //            try
    //            {
    //                var smtpSettings = _configuration.GetSection("SmtpSettings");
    //                var smtpHost = smtpSettings["Host"];
    //                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
    //                var smtpUsername = smtpSettings["Username"];
    //                var smtpPassword = smtpSettings["Password"];
    //                var fromEmail = smtpSettings["FromEmail"];

    //                using var client = new SmtpClient(smtpHost, smtpPort)
    //                {
    //                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
    //                    EnableSsl = true
    //                };

    //                using var mailMessage = new MailMessage
    //                {
    //                    From = new MailAddress(fromEmail, "Natchi Beach"),
    //                    Subject = "Membership Application Accepted - Natchi Beach",
    //                    IsBodyHtml = true,
    //                    Body = $@"
    //<html lang='en'>
    //<body style='font-family: Arial, sans-serif; font-size:14px; line-height:1.8;'>
    //    <p>Dear {memberName},</p>
    //    <p>
    //        We are pleased to inform you that your application to join the Natchi Beach membership has been accepted.
    //    </p>
    //    <p>
    //        To complete the registration process, please click the link below to finalize the payment:
    //        <br/>
    //        <a href='https://natchibaech.com/membership'
    //           style='color: #ffffff; background-color: #007bff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
    //           Complete Payment
    //        </a>
    //    </p>
    //    <p>
    //        After successful payment, you will receive a confirmation message with details of your membership.
    //    </p>
    //    <p>
    //        We welcome you to the Natchi Beach family and wish you a remarkable experience with us.
    //    </p>
    //    <p>
    //        Best regards, <br/>
    //        Natchi Beach Team
    //    </p>
    //</body>
    //</html>"
    //                };

    //                mailMessage.To.Add(email);

    //                await client.SendMailAsync(mailMessage);
    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Failed to send approval email to {Email}", email);
    //                return false;
    //            }
    //        }



    //    }

    #endregion

    public class ImgbbUploader
    {
        private static readonly string ApiKey = "6dbb3cf78613c9a6da38d809f9852a53";

        public static async Task<string> UploadImageAsync(byte[] imageBytes, string fileName = "qrcode.png")
        {
            var apiUrl = $"https://api.imgbb.com/1/upload?key={ApiKey}";

            using (var httpClient = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(imageBytes, 0, imageBytes.Length), "image", fileName);

                var response = await httpClient.PostAsync(apiUrl, form);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.Linq.JObject.Parse(jsonResponse);

                if (result["success"].Value<bool>())
                {
                    return result["data"]["url"].Value<string>();
                }
                else
                {
                    throw new Exception(result["error"]["message"].Value<string>());
                }
            }
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #region Helpers
        private MimeMessage CreateEmailMessage(string fromEmail, string fromName, string toEmail, string subject, string htmlBody, byte[]? qrCodeBytes = null, string? ticketNumber = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            // Attach QR code if exists
            if (qrCodeBytes != null && !string.IsNullOrEmpty(ticketNumber))
            {
                var attachment = builder.Attachments.Add($"ticket-{ticketNumber}.png", qrCodeBytes, new ContentType("image", "png"));
                attachment.ContentId = MimeUtils.GenerateMessageId();
            }

            message.Body = builder.ToMessageBody();
            return message;
        }

        private async Task<bool> SendAsync(MimeMessage message)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MailKit: Failed to send email to {Recipients}", string.Join(", ", message.To));
                return false;
            }
        }
        #endregion


        #region Ticket Emails
        public async Task<bool> SendSingleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string memberssn)
        {
            var fromEmail = _configuration["SmtpSettings:FromEmail"] ?? "noreply@ticketing.com";
            var qrBytes = Convert.FromBase64String(qrCodeBase64);
            var qrCodeUrl = await ImgbbUploader.UploadImageAsync(qrBytes, $"{ticketNumber}.png");
            Console.WriteLine($"<img src='{qrCodeUrl}'>");
            var htmlBody = GenerateTicketEmailTemplateWithoutPartner(ticketNumber, memberName, email, phone, bookingDate, expiryDate, qrCodeUrl,memberssn);

            var message = CreateEmailMessage(fromEmail, "Ticketing System", email, $"Your Ticket - {ticketNumber}", htmlBody, qrBytes, ticketNumber);

            return await SendAsync(message);
        }

        public async Task<bool> SendCoupleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string partnerName, string partnerEmail, string partnerPhone,string partnerId, string memberssn)
        {
            var fromEmail = _configuration["SmtpSettings:FromEmail"] ?? "noreply@ticketing.com";
            var qrBytes = Convert.FromBase64String(qrCodeBase64);
            var qrCodeUrl = await ImgbbUploader.UploadImageAsync(qrBytes, $"{ticketNumber}.png");
            Console.WriteLine($"<img src='{qrCodeUrl}'>");
            var htmlBody = GenerateTicketEmailTemplateWithPartner(ticketNumber, memberName, email, phone, partnerName, partnerEmail, partnerPhone, partnerId, bookingDate, expiryDate, qrCodeUrl,memberssn);

            var message = CreateEmailMessage(fromEmail, "Ticketing System", email, $"Your Ticket - {ticketNumber}", htmlBody, qrBytes, ticketNumber);

            return await SendAsync(message);
        }
        #endregion


        #region Membership Emails
        public async Task<bool> SendRejectedEmail(string email, string memberName)
        {
            var fromEmail = _configuration["SmtpSettings:FromEmail"] ?? "noreply@natchibeach.com";

            var body = $@"
<html lang='en'>
<body style='font-family: Arial, sans-serif; font-size:14px; line-height:1.8;'>
    <p>Dear {memberName},</p>
    <p>Thank you for your interest in joining the Natchi Beach membership.</p>
    <p>We apologize, but the number of members has currently reached its limit.</p>
    <p>Thank you for your understanding and interest.</p>
    <p>Best regards, <br/>Natchi Beach Team</p>
</body>
</html>";

            var message = CreateEmailMessage(fromEmail, "Natchi Beach", email, "Membership Application Update - Natchi Beach", body);
            return await SendAsync(message);
        }

        public async Task<bool> SendApprovedEmail(string email, string memberName)
        {
            var fromEmail = _configuration["SmtpSettings:FromEmail"] ?? "noreply@natchibeach.com";

            var body = $@"
<html lang='en'>
<body style='font-family: Arial, sans-serif; font-size:14px; line-height:1.8;'>
    <p>Dear {memberName},</p>
    <p>We are pleased to inform you that your application to join the Natchi Beach membership has been accepted.</p>
    <p>
        To complete the registration process, please click the link below to finalize the payment:
        <br/>
        <a href='https://natchibaech.com/membership'
           style='color: #ffffff; background-color: #007bff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
           Complete Payment
        </a>
    </p>
    <p>After successful payment, you will receive a confirmation message with details of your membership.</p>
    <p>We welcome you to the Natchi Beach family and wish you a remarkable experience with us.</p>
    <p>Best regards, <br/>Natchi Beach Team</p>
</body>
</html>";

            var message = CreateEmailMessage(fromEmail, "Natchi Beach", email, "Membership Application Accepted - Natchi Beach", body);
            return await SendAsync(message);
        }
        #endregion


        #region Template methods

        private static string GenerateTicketEmailTemplateWithPartner(
            string ticketNumber,
            string memberName,
            string email,
            string phone,
            string partnerName,
            string partnerEmail,
            string partnerPhone,
            string partnerId,
            DateTime bookingDate,
            DateTime expiryDate,
            string qrCodeBase64,
            string memberssn)
        {
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
                max-width: 950px;
                margin: 0 auto;
                background-color: #fff;
                border-radius: 12px;
                box-shadow: 0 2px 12px rgba(0,0,0,0.1);
                display: flex;
                gap: 20px;
                padding: 20px;
            }}
            .card {{
                flex: 1;
                border-radius: 12px;
                padding: 20px;
                background: #fff;
            }}
            .left-header {{
                height: 100px;
                background: linear-gradient(90deg, #ff6600, #ff8c1a);
                border-radius: 12px 12px 0 0;
                display: flex;
                align-items: center;
                justify-content: center;
            }}
            .info {{
                margin-top: 15px;
                font-size: 14px;
            }}
            .info p {{
                margin: 6px 0;
            }}
            .qr-code {{
                margin-top: 20px;
                text-align: center;
            }}
            .benefits ul {{
                list-style: none;
                padding: 0;
            }}
            .benefits li {{
                margin: 8px 0;
                font-size: 14px;
            }}
            .download-btn {{
                display: inline-block;
                background-color: #ff6600;
                color: #fff;
                padding: 12px 20px;
                border-radius: 8px;
                text-decoration: none;
                margin: 15px 0;
                font-weight: bold;
            }}
            .wallet-buttons {{
                display: flex;
                justify-content: center;
                gap: 10px;
                margin-top: 15px;
            }}
            .wallet-buttons img {{
                height: 40px;
            }}
          </style>
        </head>
        <body>
          <div class='container'>

            <!-- LEFT CARD -->
            <div class='card'>
              <div class='left-header'>
                <img src='https://images2.imgbox.com/59/b4/L3jpwh9t_o.png' alt='Logo' width='300'/>
              </div>
              <div class='info'>
                <p><strong>Member name:</strong> {memberName}</p>
                <p><strong>ID number:</strong> {memberssn}</p>

                <p><strong>Email:</strong> {email}</p>
                <p><strong>Phone:</strong> {phone}</p>
                <p><strong>Partner name:</strong> {partnerName}</p>
                <p><strong>Partner Phone:</strong> {partnerPhone}</p>
                <p><strong>Partner Email:</strong> {partnerEmail}</p>

                <p><strong>Partner Id:</strong> {partnerId}</p>
                <p><strong>serial  number: </strong> {ticketNumber}</p>
                <p><strong>Booking day:</strong> {bookingDate:dd/MM/yyyy}</p>
                <p><strong>Expiry Booking:</strong> {expiryDate:dd/MM/yyyy}</p>
              </div>
              <div class='qr-code'>
                <img src='{qrCodeBase64}' width='120' alt='QR Code'/>
              </div>
            </div>

            <!-- RIGHT CARD -->
            <div class='card'>
              <h2 style='text-align:center;'>Digital member card</h2>
              <p style='text-align:center;'>Membership Benefits:</p>
              <p style='text-align:center;'>Unlimited access to the resort throughout the year</p>

              <div class='benefits'>
                <ul>
                  <li>Visit vip place</li>
                  <li>Exclusive discounts on food and beverages</li>
                  <li>Priority booking for members-only Natchi events</li>
                  <li>VIP seating reservations at the beach and club areas</li>
                </ul>
              </div>

              <div style='text-align:center;'>
                <a href='#' class='download-btn'>Download membership card</a>
              </div>
            </div>

          </div>
        </body>
        </html>";
        }

        // ===== METHOD 2: Without Partner Name =====
        private static string GenerateTicketEmailTemplateWithoutPartner(
            string ticketNumber,
            string memberName,
            string email,
            string phone,
            DateTime bookingDate,
            DateTime expiryDate,
            string qrCodeBase64,string memberssn)
        {
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
                max-width: 950px;
                margin: 0 auto;
                background-color: #fff;
                border-radius: 12px;
                box-shadow: 0 2px 12px rgba(0,0,0,0.1);
                display: flex;
                gap: 20px;
                padding: 20px;
            }}
            .card {{
                flex: 1;
                border-radius: 12px;
                padding: 20px;
                background: #fff;
            }}
            .left-header {{
                height: 100px;
                background: linear-gradient(90deg, #ff6600, #ff8c1a);
                border-radius: 12px 12px 0 0;
                display: flex;
                align-items: center;
                justify-content: center;
            }}
            .info {{
                margin-top: 15px;
                font-size: 14px;
            }}
            .info p {{
                margin: 6px 0;
            }}
            .qr-code {{
                margin-top: 20px;
                text-align: center;
            }}
            .benefits ul {{
                padding: 0;
            }}
            .benefits li {{
                margin: 8px 0;
                font-size: 14px;
            }}
            .download-btn {{
                display: inline-block;
                background-color: #ff6600;
                color: #fff;
                padding: 12px 20px;
                border-radius: 8px;
                text-decoration: none;
                margin: 15px 0;
                font-weight: bold;
            }}
            .wallet-buttons {{
                display: flex;
                justify-content: center;
                gap: 10px;
                margin-top: 15px;
            }}
            .wallet-buttons img {{
                height: 40px;
            }}
          </style>
        </head>
        <body>
          <div class='container'>

            <!-- LEFT CARD -->
            <div class='card'>
              <div class='left-header'>
                <img src='https://images2.imgbox.com/59/b4/L3jpwh9t_o.png' alt='Logo' width='300'/>
              </div>
              <div class='info'>
                <p><strong>Member name:</strong> {memberName}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Phone:</strong> {phone}</p>
                <p><strong>ID number:</strong> {memberssn}</p>
                <p><strong>serial  number: :</strong> {ticketNumber}</p>
                <p><strong>Booking day:</strong> {bookingDate:dd/MM/yyyy}</p>
                <p><strong>Expiry Booking:</strong> {expiryDate:dd/MM/yyyy}</p>
              </div>
              <div class='qr-code'>
                <img src='{qrCodeBase64}' width='120' alt='QR Code'/>
              </div>
              
            </div>

            <!-- RIGHT CARD -->
            <div class='card'>
              <h2 style='text-align:center;'>Digital member card</h2>
              <p style='text-align:center;'>Membership Benefits:</p>
              <p style='text-align:center;'>Unlimited access to the resort throughout the year</p>

              <div class='benefits'>
                <ul>
                  <li>Visit vip place</li>
                  <li>Exclusive discounts on food and beverages</li>
                  <li>Priority booking for members-only Natchi events</li>
                  <li>VIP seating reservations at the beach and club areas</li>
                </ul>
              </div>
            </div>

          </div>
        </body>
        </html>";
        }

        #endregion
    }


    public interface INotificationService
    {
        Task<bool> SendSingleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string memberssn);
        Task<bool> SendCoupleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string partnerName, string partnerEmail, string partnerPhone, string partnerId,string memberssn);
        //Task<bool> SendCoupleTicketByEmailAsync(string email, string ticketNumber, string qrCodeBase64, string memberName, DateTime bookingDate, DateTime expiryDate, string phone, string partnerName);
        //Task<bool> SendTicketByWhatsAppAsync(string phoneNumber, string ticketNumber, string qrCodeBase64, string memberName, DateTime expiryDate);
        //Task<bool> SendTicketBySMSAsync(string phoneNumber, string ticketNumber, string memberName, DateTime expiryDate);
        Task<bool> SendRejectedEmail(string email, string memberName);
        Task<bool> SendApprovedEmail(string email, string memberName);

        //Task<bool> SendEmail(string to, string subject, string body);

    }
}
