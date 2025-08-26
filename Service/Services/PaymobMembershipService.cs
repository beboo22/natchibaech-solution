using Domain.Entity;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using Travelsite.DTOs;

namespace Service.Services
{
    public class PaymobMembershipService : IPaymobMembershipService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymobMembershipService> _logger;
        private ApplicationDbContext _context;

        private string ApiKey => _configuration["PaymobSettings:ApiKey"]
            ?? throw new InvalidOperationException("Paymob API key not configured");
        private string IntegrationId => _configuration["PaymobSettings:IntegrationId"]
            ?? throw new InvalidOperationException("Paymob Integration ID not configured");
        private string HmacSecret => _configuration["PaymobSettings:Hmac"]
            ?? throw new InvalidOperationException("Paymob HMAC secret not configured");
        private string BaseUrl => "https://ksa.paymob.com/api";

        public PaymobMembershipService(HttpClient httpClient, IConfiguration configuration, ILogger<PaymobMembershipService> logger, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<string> InitiatePaymentAsync(MemberShip membership, string? callbackUrl = null, string? returnUrl = null)
        {
            try
            {
                //var item = await  _context.MemberShip
                // .Include(mc => mc.MembershipCard)
                // .Include(c=>c.User)
                // .FirstOrDefaultAsync(mc => mc.Id == membership.Id);

                // Step 1: Get authentication token
                var authToken = await GetAuthTokenAsync();

                // Step 2: Create order in Paymob for the membership
                var paymobOrder = await CreatePaymobOrderAsync(authToken, membership);

                // Step 3: Get payment key
                var paymentKey = await GetPaymentKeyAsync(authToken, paymobOrder.Id, membership);

                // Step 4: Generate payment URL
                //var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/YOUR_IFRAME_ID?payment_token={paymentKey}";
                //var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_configuration["PaymobSettings:IframeId"]}?payment_token={paymentKey}";
                var paymentUrl = $"https://ksa.paymob.com/api/acceptance/iframes/{_configuration["PaymobSettings:IframeId"]}?payment_token={paymentKey}";



                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Paymob payment for membership {MembershipNumber}", membership.MembershipNumber);
                throw new InvalidOperationException("Failed to initiate payment with Paymob", ex);
            }
        }

        private async Task<string> GetAuthTokenAsync()
        {
            // Validate ApiKey
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new InvalidOperationException("API key is missing or invalid.");
            }

            // Validate BaseUrl
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new InvalidOperationException("Base URL is missing or invalid.");
            }

            var authRequest = new { api_key = ApiKey };
            var json = JsonSerializer.Serialize(authRequest, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Request JSON: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Retry policy for transient errors (e.g., 502, 503)
            int maxRetries = 3;
            int retryDelayMs = 1000;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await _httpClient.PostAsync($"{BaseUrl}/auth/tokens", content);
                    Console.WriteLine($"API Response: Status {response.StatusCode}, Attempt {attempt}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"API Error: Status {response.StatusCode}, Content: {errorContent}");

                        // Check for transient errors
                        if (response.StatusCode == HttpStatusCode.BadGateway ||
                            response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            if (attempt < maxRetries)
                            {
                                Console.WriteLine($"Retrying in {retryDelayMs}ms...");
                                await Task.Delay(retryDelayMs);
                                retryDelayMs *= 2; // Exponential backoff
                                continue;
                            }
                            throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {errorContent} after {maxRetries} attempts");
                        }

                        // Handle other errors
                        try
                        {
                            var errorResponse = JsonSerializer.Deserialize<PaymobErrorResponse>(
                                errorContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                            throw new HttpRequestException($"Paymob API error {errorResponse?.Status}: {errorResponse?.Detail}");
                        }
                        catch (JsonException)
                        {
                            throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {errorContent}");
                        }
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {responseContent}");
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var authResponse = JsonSerializer.Deserialize<PaymobAuthResponse>(responseContent, options);
                    return authResponse?.Token ?? throw new InvalidOperationException("Failed to get auth token from Paymob");
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"Request failed: {ex.Message}. Retrying in {retryDelayMs}ms...");
                    await Task.Delay(retryDelayMs);
                    retryDelayMs *= 2; // Exponential backoff
                }
            }

            throw new InvalidOperationException($"Failed to get auth token from Paymob after {maxRetries} attempts");
        }

        private async Task<PaymobOrderResponse> CreatePaymobOrderAsync(string authToken, MemberShip membership, string? callbackUrl = "https://ef0318b2efaa.ngrok-free.app/api/Transactions/paymob/GenericConfirm")
        {
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(membership.MembershipCard.Price * 100),
                currency = "SAR",
                merchant_order_id = "MEM-" + Guid.NewGuid().ToString(),
                callback_url = callbackUrl ?? _configuration["PaymobSettings:CallbackUrl"],
                items = new[]
                {
            new
            {
                name = "Membership Card",
                amount_cents = (int)(membership.MembershipCard.Price * 100),
                description = $"Membership Number: {membership.MembershipNumber}, Expiry: {membership.Expiry:yyyy-MM-dd}",
                quantity = 1
            }
        }
            };

            var json = JsonSerializer.Serialize(orderRequest, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Request JSON: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/ecommerce/orders", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: Status {response.StatusCode}, Content: {responseContent}");
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<PaymobErrorResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpRequestException($"Paymob API error {errorResponse?.Status}: {errorResponse?.Detail}");
                }
                catch (JsonException)
                {
                    throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {responseContent}");
                }
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var orderResponse = JsonSerializer.Deserialize<PaymobOrderResponse>(responseContent, options);
            return orderResponse ?? throw new InvalidOperationException("Failed to create membership order in Paymob");
        }
        private async Task<string> GetPaymentKeyAsync(string authToken, int paymobOrderId, MemberShip membership)
        {
            if (!int.TryParse(IntegrationId, out var integrationId))
            {
                throw new InvalidOperationException("Invalid IntegrationId format");
            }

            var paymentKeyRequest = new
            {
                auth_token = authToken,
                amount_cents = (int)(membership.MembershipCard.Price * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = membership.User?.Email ?? "customer@example.com",
                    floor = "NA",
                    first_name = membership.User?.FullName?.Split(" ")?.FirstOrDefault() ?? "Customer",
                    street = "NA",
                    building = "NA",
                    phone_number = membership.User?.Phone ?? "966500000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "NA",
                    country = "SA", // Match currency
                    last_name = membership.User?.FullName?.Split(" ")?.LastOrDefault() ?? "User",
                    state = "NA"
                },
                currency = "SAR", // Match CreatePaymobOrderAsync
                integration_id = integrationId
            };

            var json = JsonSerializer.Serialize(paymentKeyRequest, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Request JSON: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/acceptance/payment_keys", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: Status {response.StatusCode}, Content: {responseContent}");
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<PaymobErrorResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpRequestException($"Paymob API error {errorResponse?.Status}: {errorResponse?.Detail}");
                }
                catch (JsonException)
                {
                    throw new HttpRequestException($"Paymob API returned {response.StatusCode}: {responseContent}");
                }
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var paymentKeyResponse = JsonSerializer.Deserialize<PaymobPaymentKeyResponse>(responseContent, options);
            return paymentKeyResponse?.Token ?? throw new InvalidOperationException("Failed to get payment key from Paymob");
        }

        private static string ComputeHmacSha512(string data, string key)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLower();
        }

    }

    public interface IPaymobMembershipService
    {
        public Task<string> InitiatePaymentAsync(MemberShip membership, string? callbackUrl = null, string? returnUrl = null);
        //public bool VerifyWebhook(Dictionary<string, string> receivedData, string receivedHmac);
    }
    //public async Task<bool> VerifyWebhookAsync(PaymobWebhookDto webhook, string receivedHmac)
    //{
    //    try
    //    {
    //        if (webhook.Obj == null) return false;
    //        var hmacString =
    //            $"{webhook.Obj.Amount_cents}" +
    //            $"{webhook.Obj.Id}" +
    //            $"{webhook.Obj.Order?.Id}" +
    //            $"{webhook.Obj.Success.ToString().ToLower()}" +
    //             $"{HmacSecret}"; //https://ef0318b2efaa.ngrok-free.app/api/Transactions/paymob/GenericConfirm?id=515513&pending=false&amount_cents=700000&success=true&is_auth=false&is_capture=false&is_standalone_payment=true&is_voided=false&is_refunded=false&is_3d_secure=true&integration_id=13990&profile_id=10262&has_parent_transaction=false&order=643734&created_at=2025-08-25T15%3A40%3A09.985651%2B03%3A00&currency=SAR&merchant_commission=0&accept_fees=0&discount_details=%5B%5D&is_void=false&is_refund=false&error_occured=false&refunded_amount_cents=0&captured_amount=0&updated_at=2025-08-25T15%3A40%3A26.296244%2B03%3A00&is_settled=false&bill_balanced=false&is_bill=false&owner=11791&merchant_order_id=ORD-afbabfab-d4cc-48f1-bcd8-89b6dbbdc0ad&data.message=Approved&source_data.type=card&source_data.pan=1111&source_data.sub_type=Visa&acq_response_code=00&txn_response_code=APPROVED&hmac=4a4ea0a2cc09c0a0250be710fb00fbb6f82c9490ba6775cf76f3c933627d4c31ce1383cbe419aee21bf3a9e672734f3a9e8d4ca4186246bdc43dd54e029ea92e
    //        Console.WriteLine(hmacString);
    //        var computedHmac = ComputeHmacSha512(hmacString, HmacSecret);
    //        return computedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);
    //    }
    //    catch (Exception ex) { _logger.LogError(ex, "Error verifying Paymob webhook for membership"); return false; }
    //}


    //public async Task<bool> VerifyWebhookAsync(PaymobWebhookDto webhook, string receivedHmac)
    //{
    //    try
    //    {
    //        if (webhook.Obj == null)
    //            return false;

    //        // follow Paymob docs: concatenate these in order
    //        var hmacString =
    //            $"{webhook.Obj.Amount_cents}" +
    //            $"{webhook.Obj.Created_at}" +
    //            $"{webhook.Obj.Currency}" +
    //            $"{webhook.Obj.Error_occured}" +
    //            $"{webhook.Obj.Has_parent_transaction}" +
    //            $"{webhook.Obj.Id}" +
    //            $"{webhook.Obj.Integration_id}" +
    //            $"{webhook.Obj.Is_3d_secure}" +
    //            $"{webhook.Obj.Is_auth}" +
    //            $"{webhook.Obj.Is_capture}" +
    //            $"{webhook.Obj.Is_refunded}" +
    //            $"{webhook.Obj.Is_standalone_payment}" +
    //            $"{webhook.Obj.Is_voided}" +
    //            $"{webhook.Obj.Order?.Id}" +
    //            $"{webhook.Obj.Owner}" +
    //            $"{webhook.Obj.Pending}" +
    //            $"{webhook.Obj.Source_data?.Pan}" +
    //            $"{webhook.Obj.Source_data?.Sub_type}" +
    //            $"{webhook.Obj.Source_data?.Type}" +
    //            $"{webhook.Obj.Success.ToString().ToLower()}";

    //        var computedHmac = ComputeHmacSha512(hmacString, HmacSecret);

    //        return computedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error verifying Paymob webhook");
    //        return false;
    //    }
    //}

    //public TransactionStatus MapPaymobStatusToTransactionStatus(bool success, string? responseCode)
    //{
    //    if (success && responseCode == "APPROVED")
    //        return TransactionStatus.Success;

    //    if (!success)
    //        return TransactionStatus.Failed;

    //    return TransactionStatus.Pending;
    //}
}
