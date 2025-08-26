using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TicketingSystem.Services;

namespace Service.Services
{
    #region test01 in package

    //public class PaymobMembershipService : IPaymobMembershipService
    //{

    //    private readonly HttpClient _httpClient;
    //    private readonly Paymob.Net.IPaymobService _paymobClient;
    //    private readonly IConfiguration _configuration;
    //    private readonly ILogger<PaymobMembershipService> _logger;
    //    private readonly ApplicationDbContext _context;
    //    private string BaseUrl => "https://ksa.paymob.com/api";
    //    private string ApiKey => _configuration["PaymobSettings:ApiKey"] ?? throw new InvalidOperationException("Paymob API key not configured");

    //    private string IntegrationId => _configuration["PaymobSettings:IntegrationId"]
    //        ?? throw new InvalidOperationException("Paymob Integration ID not configured");
    //    private string HmacSecret => _configuration["PaymobSettings:Hmac"]
    //        ?? throw new InvalidOperationException("Paymob HMAC secret not configured");

    //    public PaymobMembershipService(
    //        Paymob.Net.IPaymobService paymobClient,
    //        IConfiguration configuration,
    //        ILogger<PaymobMembershipService> logger,
    //        ApplicationDbContext context,
    //        HttpClient httpClient)
    //    {
    //        _paymobClient = paymobClient;
    //        _configuration = configuration;
    //        _logger = logger;
    //        _context = context;
    //        _httpClient = httpClient;
    //    }

    //    public async Task<string> InitiatePaymentAsync(MemberShip membership)
    //    {
    //        try
    //        {
    //            // 1. Create order
    //            var orderRequest = new Paymob.Net.Models.OrderRegistrationRequest
    //            {
    //                AuthToken = await GetAuthTokenAsync(),
    //                AmountCents = $"{(int)(membership.MembershipCard.Price * 100)}",
    //                Currency = "SAR",
    //                MerchantOrderId = $"{"MEM-" + Guid.NewGuid()}",//solve i need to send it to paymob
    //                Items = new List<Paymob.Net.Models.OrderItem>
    //                        {
    //                            new Paymob.Net.Models.OrderItem
    //                            {
    //                                Name = "Membership Card",
    //                                AmountCents = (int)(membership.MembershipCard.Price * 100),
    //                                Quantity = 1,
    //                                Description = $"Membership {membership.MembershipNumber}, Expiry {membership.Expiry:yyyy-MM-dd}"
    //                            }
    //                        }
    //            };
    //            Paymob.Net.Models.OrderRegistrationResponse order = await _paymobClient.RegisterOrderAsync(orderRequest);

    //            // 2. Request payment key
    //            var billing = new BillingData
    //            {
    //                FirstName = membership.User?.FullName?.Split(" ").FirstOrDefault() ?? "Customer",
    //                LastName = membership.User?.FullName?.Split(" ").LastOrDefault() ?? "User",
    //                Email = membership.User?.Email ?? "customer@example.com",
    //                PhoneNumber = membership.User?.Phone ?? "966500000000",
    //                Country = "SA",
    //                City = "NA",
    //                Street = "NA",
    //                Building = "NA",
    //                Floor = "NA",
    //                Apartment = "NA"
    //            };

    //            var paymentKeyRequest = new PaymentKeyRequest
    //            {
    //                OrderId = order.Id,
    //                IntegrationId = int.Parse(IntegrationId),
    //                AmountCents = (int)(membership.MembershipCard.Price * 100),
    //                Currency = "SAR",
    //                BillingData = billing,
    //                Expiration = 3600
    //            };

    //            PaymentKeyResponse paymentKey = await _paymobClient.RequestPaymentKeyAsync(paymentKeyRequest);

    //            // 3. Build iframe URL
    //            return $"https://ksa.paymob.com/acceptance/iframes/{_configuration["PaymobSettings:IframeId"]}?payment_token={paymentKey.Token}";
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error initiating Paymob payment for membership {MembershipNumber}", membership.MembershipNumber);
    //            throw;
    //        }
    //    }

    //    //public bool VerifyWebhook(Dictionary<string, string> receivedData, string receivedHmac)
    //    //{
    //    //    try
    //    //    {
    //    //        // Paymob.Net has built-in helper for HMAC validation
    //    //        return _paymobClient.v.Verify(receivedData, HmacSecret, receivedHmac);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        _logger.LogError(ex, "Error verifying Paymob webhook");
    //    //        return false;
    //    //    }
    //    //}

    //    private async Task<string> GetAuthTokenAsync()
    //    {
    //        // Validate ApiKey
    //        if (string.IsNullOrWhiteSpace(ApiKey))
    //        {
    //            throw new InvalidOperationException("API key is missing or invalid.");
    //        }

    //        // Validate BaseUrl
    //        if (string.IsNullOrWhiteSpace(BaseUrl))
    //        {
    //            throw new InvalidOperationException("Base URL is missing or invalid.");
    //        }

    //        var authRequest = new { api_key = ApiKey };
    //        var json = JsonSerializer.Serialize(authRequest, new JsonSerializerOptions { WriteIndented = true });
    //        Console.WriteLine($"Request JSON: {json}");
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        // Retry policy for transient errors (e.g., 502, 503)
    //        int maxRetries = 3;
    //        int retryDelayMs = 1000;
    //        for (int attempt = 1; attempt <= maxRetries; attempt++)
    //        {
    //            try
    //            {
    //                var response = await _httpClient.PostAsync($"{BaseUrl}/auth/tokens", content);
    //                Console.WriteLine($"API Response: Status {response.StatusCode}, Attempt {attempt}");

    //                if (!response.IsSuccessStatusCode)
    //                {
    //                    var errorContent = await response.Content.ReadAsStringAsync();
    //                    Console.WriteLine($"API Error: Status {response.StatusCode}, Content: {errorContent}");

    //                    // Check for transient errors
    //                    if (response.StatusCode == HttpStatusCode.BadGateway ||
    //                        response.StatusCode == HttpStatusCode.ServiceUnavailable)
    //                    {
    //                        if (attempt < maxRetries)
    //                        {
    //                            Console.WriteLine($"Retrying in {retryDelayMs}ms...");
    //                            await Task.Delay(retryDelayMs);
    //                            retryDelayMs *= 2; // Exponential backoff
    //                            continue;
    //                        }
    //                        throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {errorContent} after {maxRetries} attempts");
    //                    }

    //                    // Handle other errors
    //                    try
    //                    {
    //                        var errorResponse = JsonSerializer.Deserialize<PaymobErrorResponse>(
    //                            errorContent,
    //                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //                        );
    //                        throw new HttpRequestException($"Paymob API error {errorResponse?.Status}: {errorResponse?.Detail}");
    //                    }
    //                    catch (JsonException)
    //                    {
    //                        throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {errorContent}");
    //                    }
    //                }

    //                var responseContent = await response.Content.ReadAsStringAsync();
    //                Console.WriteLine($"Response Content: {responseContent}");
    //                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    //                var authResponse = JsonSerializer.Deserialize<PaymobAuthResponse>(responseContent, options);
    //                return authResponse?.Token ?? throw new InvalidOperationException("Failed to get auth token from Paymob");
    //            }
    //            catch (HttpRequestException ex) when (attempt < maxRetries)
    //            {
    //                Console.WriteLine($"Request failed: {ex.Message}. Retrying in {retryDelayMs}ms...");
    //                await Task.Delay(retryDelayMs);
    //                retryDelayMs *= 2; // Exponential backoff
    //            }
    //        }

    //        throw new InvalidOperationException($"Failed to get auth token from Paymob after {maxRetries} attempts");
    //    }



    //}

    #endregion

    // Assuming this is used with ApplicationDbContext

    // Assuming the following types exist in your project:
    // public class MemberShip { public string MembershipNumber { get; set; } public MemberShipCard MembershipCard { get; set; } public DateTime Expiry { get; set; } public ApplicationUser User { get; set; } }
    // public class MemberShipCard { public decimal Price { get; set; } }
    // public class ApplicationUser { public string FullName { get; set; } public string Email { get; set; } public string Phone { get; set; } }
    // public class ApplicationDbContext : DbContext { ... }


    public class PaymobMembershipServiceNew : IPaymobMembershipServiceNew
    {
        //private readonly Paymob.Net.IPaymobService _paymobClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymobMembershipServiceNew> _logger;
        private readonly ApplicationDbContext _context;

        // Use IntegrationId and HmacSecret accessors
        private int IntegrationId => int.Parse(_configuration["PaymobSettings:IntegrationId"]
            ?? throw new InvalidOperationException("Paymob Integration ID not configured"));
        private string HmacSecret => _configuration["PaymobSettings:Hmac"]
            ?? throw new InvalidOperationException("Paymob HMAC secret not configured");
        private string IframeId => _configuration["PaymobSettings:IframeId"]
            ?? throw new InvalidOperationException("Paymob Iframe ID not configured");

        private readonly ITransactionService _transactionService;

        public PaymobMembershipServiceNew(
            //Paymob.Net.IPaymobService paymobClient,
            IConfiguration configuration,
            ILogger<PaymobMembershipServiceNew> logger,
            ApplicationDbContext context,
            ITransactionService transactionService)
        {
            //_paymobClient = paymobClient;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _transactionService = transactionService;
            // HttpClient is no longer needed directly if Paymob.Net.IPaymobService handles API calls
        }

        //public async Task<string> InitiatePaymentAsync(MemberShip membership)
        //{
        //    try
        //    {
        //        // 1. Create order
        //        var orderRequest = new OrderRegistrationRequest
        //        {
        //            // AuthToken is handled internally by _paymobClient, so we remove the explicit GetAuthTokenAsync call
        //            AmountCents = $"{(int)(membership.MembershipCard.Price * 100)}",
        //            Currency = "SAR",
        //            //i = $"{"MEM-" + membership.MembershipNumber}", // Use a stable ID related to your membership
        //            Items = new List<Paymob.Net.Models.OrderItem>
        //        {
        //            new Paymob.Net.Models.OrderItem
        //            {
        //                Name = "Membership Card",
        //                AmountCents = (int)(membership.MembershipCard.Price * 100),
        //                Quantity = 1,
        //                Description = $"Membership {membership.MembershipNumber}, Expiry {membership.Expiry:yyyy-MM-dd}"
        //            }
        //        }
        //        };

        //        // RegisterOrderAsync handles the authentication internally
        //        OrderRegistrationResponse order = await _paymobClient.RegisterOrderAsync(orderRequest);

        //        // 2. Request payment key
        //        var billing = new BillingData
        //        {
        //            FirstName = membership.User?.FullName?.Split(" ").FirstOrDefault() ?? "Customer",
        //            LastName = membership.User?.FullName?.Split(" ").LastOrDefault() ?? "User",
        //            Email = membership.User?.Email ?? "customer@example.com",
        //            PhoneNumber = membership.User?.Phone ?? "966500000000",
        //            Country = "SA",
        //            // Simplification for required fields
        //            City = "NA",
        //            Street = "NA",
        //            Building = "NA",
        //            Floor = "NA",
        //            Apartment = "NA"
        //        };

        //        var paymentKeyRequest = new PaymentKeyRequest
        //        {
        //            OrderId = order.Id,
        //            IntegrationId = IntegrationId,
        //            AmountCents = (int)(membership.MembershipCard.Price * 100),
        //            Currency = "SAR",
        //            BillingData = billing,
        //            Expiration = 3600, // 1 hour expiration
        //                               // Set redirection/callback URLs here if the library supports it, otherwise configure in Paymob dashboard
        //        };

        //        PaymentKeyResponse paymentKey = await _paymobClient.RequestPaymentKeyAsync(paymentKeyRequest);

        //        // 3. Build iframe URL
        //        return $"https://ksa.paymob.com/acceptance/iframes/{IframeId}?payment_token={paymentKey.Token}";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error initiating Paymob payment for membership {MembershipNumber}", membership.MembershipNumber);
        //        throw;
        //    }
        //}

        //// New method for handling Paymob Webhooks
        //// Inside PaymobMembershipService
        //public async Task ProcessPaymobWebhookAsync(Dictionary<string, string> receivedData, string receivedHmac)
        //{
        //    try
        //    {
        //        // 1. Verify the HMAC signature.
        //        //if (!_paymobClient.Verify(receivedData, HmacSecret, receivedHmac))
        //        //{
        //        //    _logger.LogWarning("Paymob Webhook HMAC verification failed for order: {MerchantOrderId}", receivedData.GetValueOrDefault("merchant_order_id"));
        //        //    throw new UnauthorizedAccessException("Invalid HMAC signature.");
        //        //}

        //        // 2. Extract key payment status information.
        //        bool isSuccess = receivedData.GetValueOrDefault("success")?.ToLower() == "true";
        //        string merchantOrderId = receivedData.GetValueOrDefault("merchant_order_id");
        //        string transactionReference = receivedData.GetValueOrDefault("id");

        //        if (string.IsNullOrEmpty(merchantOrderId))
        //        {
        //            _logger.LogWarning("Webhook received with missing merchant_order_id.");
        //            throw new ArgumentException("Missing merchant_order_id.");
        //        }

        //        //var status = _paymobClient.ProcessPaymentAsync().MapPaymobStatusToTransactionStatus(
        //        //    webhook.Obj!.Success,
        //        //    ""
        //        //);
        //        // 3. Map status and process transaction based on merchant ID prefix.
        //        if (merchantOrderId.StartsWith("ORD-"))
        //        {
        //            //var orderId = int.Parse(merchantOrderId.Replace("ORD-", ""));
        //            //var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
        //            //var pendingTransaction = transactions.FirstOrDefault(t => t.Status == TransactionStatus.Pending);
        //            //if (pendingTransaction == null)
        //            //    return new Exception("No pending transaction found for this order" );

        //            //await _transactionService.ConfirmOrderTransactionAsync(pendingTransaction.TransactionReference, status);
        //        }
        //        else if (merchantOrderId.StartsWith("MEM-"))
        //        {
        //            await ProcessMembershipTransactionAsync(merchantOrderId, isSuccess, transactionReference);
        //        }
        //        else
        //        {
        //            _logger.LogError("Unknown merchant_order_id format: {MerchantOrderId}", merchantOrderId);
        //            throw new ArgumentException("Unknown merchant_order_id format.");
        //        }

        //        _logger.LogInformation("Paymob webhook for {MerchantOrderId} processed successfully. Status: {IsSuccess}", merchantOrderId, isSuccess);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error processing Paymob webhook for merchantOrderId: {MerchantOrderId}", receivedData.GetValueOrDefault("merchant_order_id"));
        //        throw; // Re-throw to allow the controller to return a 500 status.
        //    }
        //}

        //private async Task ProcessMembershipTransactionAsync(string merchantOrderId, bool isSuccess, string paymobTransactionId)
        //{
        //    // Extract membershipId from the merchantOrderId (e.g., "MEM-123" -> 123).
        //    if (!int.TryParse(merchantOrderId.Replace("MEM-", ""), out int membershipId))
        //    {
        //        _logger.LogError("Invalid Membership ID format in merchant_order_id: {MerchantOrderId}", merchantOrderId);
        //        return;
        //    }

        //    var transaction = await _context.Transactions
        //        .FirstOrDefaultAsync(t => t.MemberShipId == membershipId && t.Status == TransactionStatus.Pending);

        //    if (transaction == null)
        //    {
        //        _logger.LogWarning("No pending transaction found for membership ID: {MembershipId}", membershipId);
        //        return;
        //    }

        //    if (isSuccess)
        //    {
        //        transaction.Status = TransactionStatus.Success;
        //    }
        //    else
        //    {
        //        transaction.Status = TransactionStatus.Failed;
        //    }

        //    // Assign the transaction reference from Paymob
        //    transaction.TransactionReference = paymobTransactionId;
        //    await _context.SaveChangesAsync();
        //}

        

        // Add a similar method for orders if needed.
        // private async Task ProcessOrderTransactionAsync(...) { ... }


    }

    public interface IPaymobMembershipServiceNew
    {
        //Task ProcessPaymobWebhookAsync(Dictionary<string, string> receivedData, string receivedHmac);
    }

}
