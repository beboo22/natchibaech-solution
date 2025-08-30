using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Travelsite.DTOs;

namespace TicketingSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymobService _paymobOrderService;
        private readonly IOrderService _orderService;
        private readonly IMembershipService _memberShipService;

        //private IPaymobMembershipServiceOld _paymobMembershipService;

        public TransactionService(ApplicationDbContext context, IPaymobService paymobService, IOrderService orderService, IMembershipService memberShipService)
        {
            _context = context;
            _paymobOrderService = paymobService;
            _orderService = orderService;
            _memberShipService = memberShipService;
            //_paymobMembershipService = paymobMembershipService;
        }

        public async Task<TransactionDto> InitiateTransactionAsync(int orderId, decimal amount)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order is not in pending status");

            // Create transaction record
            var transaction = new Transaction
            {
                OrderId = orderId,
                Amount = amount,
                Status = TransactionStatus.Pending,
                TransactionDate = DateTime.UtcNow,
                TransactionReference = GenerateTransactionReference()
            };

            _context.Transactions.Add(transaction);

            try
            {
                // Initiate payment with Paymob
                var paymentUrl = await _paymobOrderService.InitiatePaymentAsync(order, "https://ef0318b2efaa.ngrok-free.app/api/Transactions/paymob/GenericConfirm");
            await _context.SaveChangesAsync();

                // You might want to store the payment URL or additional Paymob data
                // For now, we'll return the transaction with the reference

                return new TransactionDto
                {
                    Id = transaction.Id,
                    OrderId = transaction.OrderId,
                    Amount = transaction.Amount,
                    Status = transaction.Status,
                    TransactionDate = transaction.TransactionDate,
                    TransactionReference = transaction.TransactionReference,
                    PaymentUrl = paymentUrl,
                };
            }
            catch (Exception ex)
            {
                // Update transaction status to failed
                transaction.Status = TransactionStatus.Failed;
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Failed to initiate payment", ex);
            }
        }

        public async Task<TransactionDto> MemberShipInitiateTransactionAsync(int MemberShipId, decimal amount)
        {
            

            var order = await _context.MemberShips
                .Include(m => m.MembershipCard)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == MemberShipId);

            if (order == null)
                throw new ArgumentException("MemberShip Card not found");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("MemberShip is not in pending status");


            // Create transaction record
            var transaction = new Transaction
            {
                MemberShipId = order.Id,
                Amount = order.MembershipCard.Price,
                Status = TransactionStatus.Pending,
                TransactionDate = DateTime.UtcNow,
                TransactionReference = GenerateTransactionReference()
            };

            _context.Transactions.Add(transaction);

            
            try
            {
                // Initiate payment with Paymob
                var paymentUrl = await _paymobOrderService.InitiatePaymentAsync(order, "https://ef0318b2efaa.ngrok-free.app/api/Transactions/paymob/GenericConfirm");
                await _context.SaveChangesAsync();

                // You might want to store the payment URL or additional Paymob data
                // For now, we'll return the transaction with the reference
                
                return new TransactionDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    Status = transaction.Status,
                    TransactionDate = transaction.TransactionDate,
                    TransactionReference = transaction.TransactionReference,
                    PaymentUrl = paymentUrl,
                };
            }
            catch (Exception ex)
            {
                // Update transaction status to failed
                //transaction.Status = TransactionStatus.Failed;
                throw new InvalidOperationException($"Failed to initiate payment:{ ex.Message} ", ex);
                //await _context.SaveChangesAsync();
            }
        }

        public async Task<Transaction?> ConfirmMemberShipTransactionAsync(string transactionReference, TransactionStatus status)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionReference == transactionReference);

            if (transaction == null)
                return null;

            transaction.Status = status;

            // If payment is successful, update order status
            if (status == TransactionStatus.Success)
            {
                if(transaction.MemberShipId.HasValue)
                    await _memberShipService.UpdateMembershipStatusAsync(transaction.MemberShipId.Value, OrderStatus.Paid);
            }

            await _context.SaveChangesAsync();
            return transaction;
        }
        public async Task<Transaction?> ConfirmOrderTransactionAsync(string transactionReference, TransactionStatus status)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.TransactionReference == transactionReference);

            if (transaction == null)
                return null;

            transaction.Status = status;

            // If payment is successful, update order status
            if (status == TransactionStatus.Success)
            {
                if(transaction.OrderId.HasValue)
                    await _orderService.UpdateOrderStatusAsync(transaction.OrderId.Value , OrderStatus.Paid);
            }

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByOrderIdAsync(int orderId)
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByMemberShipIdAsync(int MembershipCardId)
        {
            return await _context.Transactions
                .Include(t => t.MemberShip)
                .Where(t => t.MemberShipId == MembershipCardId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByReferenceAsync(string transactionReference)
        {
            return await _context.Transactions
                .Include(t => t.MemberShip)
                    .ThenInclude(u => u.User)
                .Include(t => t.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(t => t.TransactionReference == transactionReference);
        }

        private static string GenerateTransactionReference()
        {
            return $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}"[..32];
        }

        
    }
}
