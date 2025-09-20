using Domain.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        //private readonly string _hmacSecret = "396ED95BE69D71C17ADDEB1DBEF9747E";
        private readonly ITransactionService _transactionService;
        //private IPaymobMembershipServiceNew _paymobServiceNew;
        private IPaymobService _paymobServiceOld;
        private ApplicationDbContext _context;

        public TransactionsController(ITransactionService transactionService, IPaymobService paymobServiceOld, ApplicationDbContext context)
        {
            _transactionService = transactionService;
            //_paymobServiceNew = paymobService;
            _paymobServiceOld = paymobServiceOld;
            _context = context;
        }

        /// <summary>
        /// Initiate new transaction and call Paymob API
        /// </summary>
        [HttpPost("Orderinitiate")]
        public async Task<ActionResult<TransactionDto>> InitiateTransaction([FromBody] InitiateTransactionDto initiateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.InitiateTransactionAsync(
                    initiateDto.OrderId,
                    initiateDto.Amount);

                //var transactionDto = MapToTransactionDto(transactiontem1);
                // use ApiResponse to return the transaction details
                if (transaction == null)
                    return NotFound(new { message = "Transaction could not be initiated" });
                return Ok(new ApiResponse<TransactionDto>(200, transaction, "Transaction initiated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while initiating the transaction", error = ex.Message });
            }
        }
        [HttpPost("MemberShipinitiate")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> InitiateTransaction([FromBody] MemberShipInitiateTransactionDto initiateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.MemberShipInitiateTransactionAsync(
                    initiateDto.MembershipId,
                    initiateDto.Amount);

                //var transactionDto = MapToTransactionDto(transaction);
                // use ApiResponse to return the transaction details
                if (transaction == null)
                    return NotFound(new { message = "Transaction could not be initiated" });

                return Ok(new ApiResponse<TransactionDto>(200, transaction, "Transaction initiated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while initiating the transaction", error = ex.Message });
            }
        }

        /// <summary>
        /// Confirm transaction status after Paymob webhook
        /// </summary>
        [HttpPost("confirmOrder")]
        public async Task<ActionResult> ConfirmTransaction([FromBody] ConfirmTransactionDto confirmDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.ConfirmOrderTransactionAsync(
                    confirmDto.TransactionReference,
                    confirmDto.Status);

                if (transaction == null)
                    return NotFound(new { message = "Transaction not found" });

                var transactionDto = MapToTransactionDto(transaction);
                // use ApiResponse to return the confirmed transaction details
                if (transactionDto == null)
                    return NotFound(new { message = "Transaction could not be confirmed" });
                return Ok(new ApiResponse<TransactionDto>(200, transactionDto, "Transaction confirmed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while confirming the transaction", error = ex.Message });
            }
        }
        /// <summary>
        /// Confirm transaction status after Paymob webhook
        /// </summary>
        [HttpPost("confirmMemberShip")]
        public async Task<ActionResult<TransactionDto>> ConfirmMemberShipTransaction([FromBody] ConfirmTransactionDto confirmDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.ConfirmMemberShipTransactionAsync(
                    confirmDto.TransactionReference,
                    confirmDto.Status);

                if (transaction == null)
                    return NotFound(new { message = "Transaction not found" });

                var transactionDto = MapToTransactionDto(transaction);
                // use ApiResponse to return the confirmed transaction details
                if (transactionDto == null)
                    return NotFound(new { message = "Transaction could not be confirmed" });
                return Ok(new ApiResponse<TransactionDto>(200, transactionDto, "Transaction confirmed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while confirming the transaction", error = ex.Message });
            }
        }

        //[HttpPost("paymob/GenericConfirm")]
        //public async Task<IActionResult> GenericConfirm()
        //{
        //    try
        //    {
        //        var rawBody = await GetRawBodyAsync(Request);
        //        if (string.IsNullOrEmpty(rawBody))
        //            return BadRequest(new { message = "Invalid payload" });

        //        var wrapper = JsonConvert.DeserializeObject<PaymobCallbackWrapper>(rawBody)
        //      ?? new PaymobCallbackWrapper();
        //        if (wrapper == null || wrapper.Obj == null)
        //            return BadRequest(new { message = "Failed to parse callback payload" });

        //        var callback = wrapper.Obj ?? new PaymobTransaction();
        //        //callback.Hmac = hmac; // inject query hmac


        //        // Extract merchant order id
        //        var merchantOrderId = callback.Order?.MerchantOrderId;
        //        if (string.IsNullOrEmpty(merchantOrderId))
        //            return BadRequest(new { message = "Missing merchant_order_id" });

        //        // Determine transaction status
        //        var status = callback.Success
        //            ? TransactionStatus.Success
        //            : TransactionStatus.Failed;

        //        // Confirm transaction based on order type
        //        var confirmResult = await ConfirmTransactionAsync(merchantOrderId, status);
        //        if (confirmResult is not null)
        //            return confirmResult;

        //        //return Ok(new { message = "Webhook processed successfully" });
        //        return Redirect("https://natchibaech.com/");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect("https://natchibaech.com/");
        //        //return StatusCode(500, new { message = "Internal error", error = ex.Message });
        //    }
        //}

        #region Myproduction

        [HttpPost("paymob/GenericConfirm")]
        public async Task<IActionResult> GenericConfirm([FromBody] PaymobWebhookDto webhook, [FromQuery] string hmac)
        {
            try
            {
                //Console.WriteLine("########################################\nhi\n####################################");



                //var txnResponseCode = HttpContext.Request.Query["txn_response_code"].ToString();
                //if(string.IsNullOrWhiteSpace(txnResponseCode))
                //    Console.WriteLine("txnResponseCode not found");
                //webhook.Obj.Txn_response_code = txnResponseCode;
                if (string.IsNullOrEmpty(hmac))
                    return BadRequest(new { message = "Missing HMAC" });

                //var isValid = await _paymobService.VerifyWebhookAsync(webhook, hmac);
                //if (!isValid)
                //    return Unauthorized(new { message = "Invalid HMAC signature" });

                var merchantOrderId = webhook.Obj?.Order?.Merchant_order_id;
                if (string.IsNullOrEmpty(merchantOrderId))
                    return BadRequest(new { message = "Missing merchant_order_id" });

                //var status = _paymobService.MapPaymobStatusToTransactionStatus(
                //    webhook.Obj!.Success,
                //    ""
                //);

                var status = TransactionStatus.Pending;
                if (webhook.Obj.Success)
                    status = TransactionStatus.Success;
                else if (!webhook.Obj.Success)
                    status = TransactionStatus.Failed;

                //return
                if (merchantOrderId.StartsWith("ORD/"))
                {
                    var orderId = int.Parse(merchantOrderId.Split("/")[2]);
                    var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
                    var pendingTransaction = transactions.FirstOrDefault(t => t.Status != TransactionStatus.Success);
                    if (pendingTransaction == null)
                        return NotFound(new { message = "No pending transaction or failed transaction found for this order" });

                    await _transactionService.ConfirmOrderTransactionAsync(pendingTransaction.TransactionReference, status);
                }
                else if (merchantOrderId.StartsWith("MEM/"))
                {
                    var membershipId = int.Parse(merchantOrderId.Split("/")[2]);
                    var transactions = await _transactionService.GetTransactionsByMemberShipIdAsync(membershipId);
                    var pendingTransaction = transactions.FirstOrDefault(t => t.Status != TransactionStatus.Success);
                    if (pendingTransaction == null)
                        return NotFound(new { message = "No pending transaction found for this membership" });

                    await _transactionService.ConfirmMemberShipTransactionAsync(pendingTransaction.TransactionReference, status);
                }
                else
                {
                    return BadRequest(new { message = "Unknown merchant_order_id format" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error", error = ex.Message });
            }
            //return Redirect("https://natchibaech.com/");
            return Ok();
        }


        [HttpGet("paymob/PaymentResult")]
        public IActionResult PaymentResult([FromQuery] string success, [FromQuery] string merchant_order_id)
        {
            return Redirect("https://natchibaech.com/payment/return");
        }




        #endregion

        [HttpPost("payTransaction")]
        public async Task<IActionResult> PayTransaction(int transactionId)
        {
            // Check if transaction exists
            if (!await _context.Transactions.AnyAsync(t => t.Id == transactionId))
            {
                return BadRequest(new { Message = "Not Found" });
            }

            // Fetch transaction with related membership
            var transaction = await _context.Transactions
                .Include(t => t.MemberShip)
                    .ThenInclude(m => m.MembershipCard)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction?.MemberShip == null)
            {
                return BadRequest(new { Message = "Membership not found for this transaction" });
            }

            // Call Paymob service
            var paymentUrl = await _paymobServiceOld.InitiatePaymentAsync(transaction.MemberShip);

            return Ok(new { PaymentUrl = paymentUrl });
        }

        /// <summary>
        /// Get transaction details by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null)
                    return NotFound(new { message = "Transaction not found" });

                var transactionDto = MapToTransactionDto(transaction);
                // use ApiResponse to return the transaction details
                if (transactionDto == null)
                    return NotFound(new { message = "Transaction details not found" });
                return Ok(new ApiResponse<TransactionDto>(200, transactionDto, "Transaction retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the transaction", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all transactions for a specific order
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByOrder(int orderId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
                var transactionDtos = transactions.Select(MapToTransactionDto);
                if (!transactionDtos.Any())
                    return NotFound(new { message = "No transactions found for this order" });
                // use ApiResponse to return the list of transactions
                return Ok(new ApiResponse<IEnumerable<TransactionDto>>(200, transactionDtos, "Transactions retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving transactions", error = ex.Message });
            }
        }

        /// <summary>
        /// Paymob webhook endpoint for payment status updates
        /// </summary>
        //[HttpPost("webhook/paymob")]
        //public async Task<ActionResult> PaymobWebhook([FromBody] PaymobWebhookDto webhook)
        //{
        //    try
        //    {
        //        // Get HMAC from headers
        //        var receivedHmac = Request.Headers["X-Paymob-Hmac"].FirstOrDefault();
        //        if (string.IsNullOrEmpty(receivedHmac))
        //            return BadRequest(new { message = "Missing HMAC header" });

        //        // Verify webhook authenticity
        //        var isValid = await _paymobService.VerifyWebhookAsync(webhook, receivedHmac);
        //        if (!isValid)
        //            return Unauthorized(new { message = "Invalid webhook signature" });

        //        if (webhook.Obj?.Order?.Merchant_order_id == null)
        //            return BadRequest(new { message = "Invalid webhook data" });

        //        // Find transaction by order ID
        //        var orderId = int.Parse(webhook.Obj.Order.Merchant_order_id);
        //        var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
        //        var pendingTransaction = transactions.FirstOrDefault(t => t.Status == TransactionStatus.Pending);

        //        if (pendingTransaction == null)
        //            return NotFound(new { message = "No pending transaction found for this order" });

        //        // Map Paymob status to our transaction status
        //        var status = _paymobService.MapPaymobStatusToTransactionStatus(
        //            webhook.Obj.Success, 
        //            webhook.Obj.Txn_response_code);

        //        // Update transaction status
        //        await _transactionService.ConfirmTransactionAsync(pendingTransaction.TransactionReference, status);

        //        return Ok(new { message = "Webhook processed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred while processing the webhook", error = ex.Message });
        //    }
        //}

        [HttpPost("webhook/paymob")]
        public async Task<ActionResult> PaymobWebhook([FromBody] PaymobWebhookDto webhook, [FromQuery(Name = "txn_response_code")] object txn_response_code)
        {
            try
            {
                // Get HMAC from headers
                var receivedHmac = Request.Headers["X-Paymob-Hmac"].FirstOrDefault();
                if (string.IsNullOrEmpty(receivedHmac))
                    return BadRequest(new { message = "Missing HMAC header" });

                // Verify webhook authenticity
                var isValid = true;//await _paymobService.VerifyWebhookAsync(webhook, receivedHmac);
                if (!isValid)
                    return Unauthorized(new { message = "Invalid webhook signature" });

                if (webhook.Obj?.Order?.Merchant_order_id == null)
                    return BadRequest(new { message = "Invalid webhook data" });

                var merchantOrderId = webhook.Obj.Order.Merchant_order_id;

                // Map Paymob status to our transaction status
                var status = TransactionStatus.Success;
                //_paymobServiceOld.MapPaymobStatusToTransactionStatus(
                //    webhook.Obj.Success,
                //    (string)txn_response_code);

                if (merchantOrderId.StartsWith("ORD-"))
                {
                    // Extract numeric order ID
                    var orderId = int.Parse(merchantOrderId.Replace("ORD-", ""));

                    var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
                    var pendingTransaction = transactions.FirstOrDefault(t => t.Status == TransactionStatus.Pending);

                    if (pendingTransaction == null)
                        return NotFound(new { message = "No pending transaction found for this order" });

                    await _transactionService.ConfirmOrderTransactionAsync(pendingTransaction.TransactionReference, status);
                }
                else if (merchantOrderId.StartsWith("MEM-"))
                {
                    // Extract numeric membership ID
                    var membershipId = int.Parse(merchantOrderId.Replace("MEM-", ""));

                    var transactions = await _transactionService.GetTransactionsByMemberShipIdAsync(membershipId);
                    var pendingTransaction = transactions.FirstOrDefault(t => t.Status == TransactionStatus.Pending);

                    if (pendingTransaction == null)
                        return NotFound(new { message = "No pending transaction found for this membership" });

                    await _transactionService.ConfirmMemberShipTransactionAsync(pendingTransaction.TransactionReference, status);
                }
                else
                {
                    return BadRequest(new { message = "Unknown merchant order ID format" });
                }

                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the webhook", error = ex.Message });
            }
        }

        //private bool IsValidHmac(PaymobTransaction callback)
        //{
        //    string Safe(object? value)
        //        => value switch
        //        {
        //            null => string.Empty,
        //            bool b => b.ToString().ToLower(),  // normalize booleans
        //            System.DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"),
        //            _ => value.ToString() ?? string.Empty
        //        };

        //    var hmacDataString = string.Join(":",
        //        Safe(callback?.AmountCents),
        //        Safe(callback?.CreatedAt),
        //        Safe(callback?.Order?.Currency),
        //        Safe(callback?.ErrorOccured),
        //        Safe(callback?.HasParentTransaction),
        //        Safe(callback?.Id),
        //        Safe(callback?.Is3dSecure),
        //        Safe(callback?.Order?.Id),
        //        Safe(callback?.IsCaptured),
        //        Safe(callback?.IsRefunded),
        //        Safe(callback?.IsStandalonePayment),
        //        Safe(callback?.IsVoided),
        //        Safe(callback?.Pending),
        //        Safe(callback?.ProfileId),
        //        Safe(callback?.Success)
        //    );

        //    var computedHmac = ComputeHmacSha256(hmacDataString, _hmacSecret);

        //    return string.Equals(computedHmac, callback?.Hmac, StringComparison.OrdinalIgnoreCase);
        //}

        //private async Task<bool> VerifyWebhookAsync(PaymobTransaction webhook, string receivedHmac)
        //{
        //    if (webhook == null) return false;

        //    try
        //    {
        //        var hmacString =
        //            $"{webhook.AmountCents}" +
        //            $"{webhook.CreatedAt}" +
        //            $"{webhook.Currency}" +
        //            $"{webhook.ErrorOccured.ToString().ToLower()}" +
        //            $"{webhook.HasParentTransaction.ToString().ToLower()}" +
        //            $"{webhook.Id}" +
        //            $"{webhook.IntegrationId}" +
        //            $"{webhook.Is3dSecure.ToString().ToLower()}" +
        //            $"{webhook.IsAuth.ToString().ToLower()}" +
        //            $"{webhook.IsCapture.ToString().ToLower()}" +
        //            $"{webhook.IsRefunded.ToString().ToLower()}" +
        //            $"{webhook.IsStandalonePayment.ToString().ToLower()}" +
        //            $"{webhook.IsVoided.ToString().ToLower()}" +
        //            $"{(webhook.Order?.Id.ToString() ?? "")}" +
        //            //$"{webhook.Owner}" +  //  usually not required
        //            $"{webhook.Pending.ToString().ToLower()}" +
        //            $"{(webhook.SourceData?.Pan ?? "")}" +
        //            $"{(webhook.SourceData?.SubType ?? "")}" +
        //            $"{(webhook.SourceData?.Type ?? "")}" +
        //            $"{webhook.Success.ToString().ToLower()}";

        //        var computedHmac = ComputeHmacSha512(hmacString, _hmacSecret);

        //        return computedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}




        private async Task<IActionResult?> ConfirmTransactionAsync(string merchantOrderId, TransactionStatus status)
        {
            if (merchantOrderId.StartsWith("ORD/"))
            {
                var orderId = int.Parse(merchantOrderId.Split("/")[2]);
                var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);

                var pendingTransaction = transactions.FirstOrDefault(t => t.Status != TransactionStatus.Success);
                if (pendingTransaction == null)
                    return NotFound(new { message = "No pending or failed transaction found for this order" });

                await _transactionService.ConfirmOrderTransactionAsync(pendingTransaction.TransactionReference, status);
            }
            else if (merchantOrderId.StartsWith("MEM/"))
            {
                var membershipId = int.Parse(merchantOrderId.Split("/")[2]);
                var transactions = await _transactionService.GetTransactionsByMemberShipIdAsync(membershipId);

                var pendingTransaction = transactions.FirstOrDefault(t => t.Status != TransactionStatus.Success);
                if (pendingTransaction == null)
                    return NotFound(new { message = "No pending transaction found for this membership" });

                await _transactionService.ConfirmMemberShipTransactionAsync(pendingTransaction.TransactionReference, status);
            }
            else
            {
                return BadRequest(new { message = "Unknown merchant_order_id format" });
            }

            return null; // Success
        }
        public static async Task<string> GetRawBodyAsync(HttpRequest request)
        {
            // Enable seeking to allow reading the stream multiple times
            request.EnableBuffering();
            // Read the stream and convert to a string
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                var body = await reader.ReadToEndAsync();
                // Reset the stream's position for later processing
                request.Body.Position = 0;
                return body;
            }
        }
        public static string ComputeHmacSha256(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public static string ComputeHmacSha512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        private static TransactionDto MapToTransactionDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                OrderId = transaction.OrderId.HasValue ? transaction.OrderId.Value : null,
                MembershipId = transaction.MemberShipId.HasValue ? transaction.MemberShipId.Value : null,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Status = transaction.Status,
                TransactionReference = transaction.TransactionReference,
                PaymentUrl = string.Empty // You can store this if needed
            };
        }
    }
}
