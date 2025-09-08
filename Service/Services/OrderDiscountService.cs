using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Services
{
    public class OrderDiscountService : IOrderDiscountService
    {
        private readonly ApplicationDbContext _context;

        public OrderDiscountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDiscountCode>> GetAllDiscountCodesAsync()
        {
            return await _context.OrderDiscountCodes
                .Include(dc => dc.Orders)
                .OrderByDescending(dc => dc.Id)
                .ToListAsync();
        }

        public async Task<OrderDiscountCode?> GetDiscountCodeByCodeAsync(string code)
        {
            return await _context.OrderDiscountCodes
                .Include(dc => dc.Orders)
                .FirstOrDefaultAsync(dc => dc.Code == code);
        }

        public async Task<OrderDiscountCode> CreateDiscountCodeAsync(OrderDiscountCode discountCode)
        {
            // Check if code already exists
            var existingCode = await GetDiscountCodeByCodeAsync(discountCode.Code);
            if (existingCode != null)
                throw new ArgumentException("Discount code already exists");


            // Ensure expiry date is in the future
            if (discountCode.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future");

            _context.OrderDiscountCodes.Add(discountCode);
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"error while saving:{ex.Message}-->{ex.InnerException}", ex.InnerException);
            }
            return discountCode;
        }

        public async Task<decimal> ApplyOrderDiscountAsync(string code, int? OrderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var discountCode = await GetDiscountCodeByCodeAsync(code);

                if (discountCode == null)
                    throw new ArgumentException("Invalid discount code");

                var validationResult = ValidateDiscountCode(discountCode);
                if (!validationResult.IsValid)
                    throw new ArgumentException(validationResult.ErrorMessage);

                // Calculate discount amount
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == OrderId);
                if(order == null || order.Status == OrderStatus.Paid)
                    throw new ArgumentException("Invalid order code");
                
                var discountAmount = (order.TotalAmount * discountCode.Percentage) / 100m;
                order.FinalAmount = order.TotalAmount - discountAmount;
                order.ApplyDiscount = true;
                
                // Update usage count
                discountCode.CurrentUsage++;
                await _context.SaveChangesAsync();
                // Commit transaction
                await transaction.CommitAsync();
                return discountAmount;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> ValidateDiscountCodeAsync(string code)
        {
            var discountCode = await GetDiscountCodeByCodeAsync(code);
            if (discountCode == null)
                return false;

            var validationResult = ValidateDiscountCode(discountCode);
            return validationResult.IsValid;
        }

        public async Task<OrderDiscountCode?> UpdateDiscountCodeAsync(int id, OrderDiscountCode updatedDiscountCode)
        {
            var existingCode = await _context.OrderDiscountCodes.FindAsync(id);
            if (existingCode == null)
                return null;

            // Check if new code already exists (if code is being changed)
            if (existingCode.Code != updatedDiscountCode.Code)
            {
                var codeExists = await _context.OrderDiscountCodes
                    .AnyAsync(dc => dc.Code == updatedDiscountCode.Code && dc.Id != id);
                if (codeExists)
                    throw new ArgumentException("Discount code already exists");
            }

            existingCode.Code = updatedDiscountCode.Code;
            existingCode.Percentage = updatedDiscountCode.Percentage;
            existingCode.MaxUsage = updatedDiscountCode.MaxUsage;
            existingCode.ExpiryDate = updatedDiscountCode.ExpiryDate;
            //existingCode.UserId = updatedDiscountCode.UserId;
            existingCode.CurrentUsage = updatedDiscountCode.CurrentUsage;
            existingCode.IsActive = updatedDiscountCode.IsActive;

            await _context.SaveChangesAsync();
            return existingCode;
        }

        public async Task<bool> DeleteDiscountCodeAsync(int id)
        {
            var discountCode = await _context.OrderDiscountCodes.FindAsync(id);
            if (discountCode == null)
                return false;

            _context.OrderDiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DiscountValidationResult ValidateDiscountCode(OrderDiscountCode discountCode)
        {
            // Check if expired
            if (discountCode.ExpiryDate <= DateTime.UtcNow)
                return new DiscountValidationResult(false, "Discount code has expired");

            // Check if usage limit reached
            if (discountCode.CurrentUsage >= discountCode.MaxUsage)
                return new DiscountValidationResult(false, "Discount code usage limit reached");

            if (!discountCode.IsActive)
                return new DiscountValidationResult(false, "Discount code Not Active");


            return new DiscountValidationResult(true, "Valid");
        }

       

        private class DiscountValidationResult
        {
            public bool IsValid { get; }
            public string ErrorMessage { get; }

            public DiscountValidationResult(bool isValid, string errorMessage)
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }
        }
    }
}
