using Domain.Entity;
using Google.Apis.Walletobjects.v1.Data;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using System.Text.Json;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
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
        public async Task<ActionResult> InitiateTransaction([FromBody] MemberShipInitiateTransactionDto initiateDto)
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

        [HttpPost("paymob/GenericConfirm")]
        public async Task<IActionResult> GenericConfirm([FromBody] PaymobWebhookDto webhook, [FromQuery] string hmac)
        {
            try
            {
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
                    status= TransactionStatus.Success;
                else if(!webhook.Obj.Success)
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

                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error", error = ex.Message });
            }
        }


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
                    .ThenInclude(m=>m.MembershipCard)
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
