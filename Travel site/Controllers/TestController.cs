using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;


namespace Travel_site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IQRCodeService _qrCodeService;

        INotificationService _notificationService;
        private readonly IGoogleWalletService3 _walletService;
        IConfiguration _configuration;
        //private IPaymobService _paymobService;

        public TestController(INotificationService notificationService, IQRCodeService qrCodeService, IGoogleWalletService3 walletService, IConfiguration configuration)
        {
            _notificationService = notificationService;
            _qrCodeService = qrCodeService;
            _walletService = walletService;
            _configuration = configuration;
            //_paymobService = paymobService;
        }

        //[HttpPost]
        //public async Task<IActionResult> TestPayment()
        //{

        //    Bdaya.Net.Paymob.
        //    //
        //    var auth = await _paymobService.AuthenticateAsync();
        //    //RegisterOrderAsync

        //    var order = await _paymobService.RegisterOrderAsync(new Paymob.Net.Models.OrderRegistrationRequest
        //    {
        //        AuthToken = auth.Token,
        //        //ApiSource = auth.Token
        //        AmountCents = "7000",
        //        Currency = "SAR",
        //        Items = new List<Paymob.Net.Models.OrderItem>
        //        {
        //            new Paymob.Net.Models.OrderItem
        //            {
        //                Name = "Membership Card",
        //                AmountCents = 7000,
        //                Quantity = 1,
        //                Description = $""
        //            }
        //        }
        //    });
        //    //RequestPaymentKeyAsync
        //    var Req = await _paymobService.RequestPaymentKeyAsync(new PaymentKeyRequest
        //    {
        //        AuthToken = auth.Token,
        //        OrderId = order.Id,
        //        Expiration = 10,
        //        Currency = "SAR",
        //        BillingData = new BillingData
        //        {
        //            Email = "tesst@gmail.com",
        //            PhoneNumber = "1234567890",
        //            LastName = "tarek",
        //            FirstName = "moo",
        //        }
        //    });

        //    return Ok($"https://ksa.paymob.com/api/acceptance/iframes/{_configuration["PaymobSettings: IframeId"]}?payment_token={Req.Token}");
        //}

        [HttpGet]
        public async Task TestEmail()
        {
            var qrCodeData = GenerateMembershipQRData("123", "beboo", UserType.male, DateTime.UtcNow.AddYears(1));
            var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
            await _notificationService.SendTicketByEmailAsync("dodotarek506@gmail.com", "1333", qrCode, "beboo", DateTime.UtcNow.AddMonths(1));
        }
        //[HttpGet("/wh")]
        //public async Task TestWhats()
        //{
        //    var qrCodeData = GenerateMembershipQRData("123", "beboo", UserCategory.Men, DateTime.UtcNow.AddYears(1));
        //    var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
        //    await _notificationService.SendTicketByWhatsAppAsync("01115472381", "1333", qrCode, "beboo", DateTime.UtcNow.AddMonths(1));
        //}

        [HttpGet("create-pass")]
        public async Task<IActionResult> CreatePass()
        {
            var testTicket = new Ticket
            {
                TicketNumber = "T12345",
                MemberName = "John Doe",
                QRCode = "QRCODE12345",
                MembershipNumber = "Seat-10",
                PurchaseDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            var saveUrl = _walletService.CreateTicketWalletLink(testTicket);
            return Ok(new { SaveUrl = saveUrl });
        }

        //[HttpPost("create-ticket-pass")]
        //public IActionResult CreateTicketPass([FromBody] Ticket ticket)
        //{
        //    try
        //    {
        //        var link = _walletService.CreateEventTicketPassAsync(ticket);
        //        return Ok(new { Link = link });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = ex.Message });
        //    }
        //}

        //[HttpPost("create-ticket-class")]
        //public async Task<IActionResult> CreateTicketClass([FromBody] EventDetails eventDetails)
        //{
        //    try
        //    {
        //        var jwt = await _walletService.CreateEventTicketClassAsync(eventDetails.EventName, eventDetails.EventLocation, eventDetails.EventDate);
        //        return Ok(new { Jwt = jwt });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = ex.Message });
        //    }
        //}











        //[HttpPost("create-pass")]
        //public async Task<IActionResult> CreatePass()
        //{
        //    // create a dummy ticket object for testing
        //    var testTicket = new Ticket
        //    {
        //        Id = 1,
        //        TicketNumber = "TEST123",
        //        MemberName = "John Doe",
        //        ExpiryDate = System.DateTime.UtcNow.AddDays(7),
        //        // add any other required properties for your Ticket entity
        //    };

        //    try
        //    {
        //        var ServiceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];
        //        var IssuerId = _configuration["GoogleWalletSettings:IssuerId"];
        //        var ClassId = _configuration["GoogleWalletSettings:ClassId"];
        //        if (ServiceAccountKeyPath == null) 
        //            return NotFound();
        //        if (IssuerId == null)
        //            return NotFound();
        //        if (ClassId == null)
        //            return NotFound();
        //        var result = new GoogleWalletIntegration(
        //            ServiceAccountKeyPath,
        //            IssuerId,
        //            ClassId,
        //            "beboo");
        //        var res = await result.CreateGenericPass();

        //        // return the JWT or Save link
        //        return Ok(new
        //        {
        //            message = "Pass created successfully",
        //            pass = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            message = "Error creating pass",
        //            error = ex.Message
        //        });
        //    }
        //}

        private static string GenerateMembershipQRData(string membershipNumber, string memberName, UserType category, DateTime time)
        {
            return $"MEMBERSHIP:{membershipNumber}|NAME:{memberName}|CATEGORY:{category}|ISSUED:{DateTime.UtcNow:yyyy-MM-dd}|Expiry Data :{time:yyyy-MM-dd}";
        }
    }


    public class EventDetails
    {
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDate { get; set; }
    }
}
