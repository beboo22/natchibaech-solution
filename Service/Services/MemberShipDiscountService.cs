using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MemberShipDiscountService:IMemberShipDiscountService
    {
        private readonly ApplicationDbContext _context;

        public MemberShipDiscountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MemberShipDiscountCode>> GetAllDiscountCodesAsync()
        {
            return await _context.MemberShipDiscountCodes
                .Include(dc => dc.MemberShip)
                .OrderByDescending(dc => dc.Id)
                .ToListAsync();
        }

        public async Task<MemberShipDiscountCode?> GetDiscountCodeByCodeAsync(string code)
        {
            return await _context.MemberShipDiscountCodes
                .Include(dc => dc.MemberShip)
                .FirstOrDefaultAsync(dc => dc.Code == code);
        }

        public async Task<MemberShipDiscountCode> CreateDiscountCodeAsync(MemberShipDiscountCode discountCode)
        {
            // Check if code already exists
            var existingCode = await GetDiscountCodeByCodeAsync(discountCode.Code);
            if (existingCode != null)
                throw new ArgumentException("Discount code already exists");


            // Ensure expiry date is in the future
            if (discountCode.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future");

            _context.MemberShipDiscountCodes.Add(discountCode);
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

        public async Task<bool> ValidateDiscountCodeAsync(string code)
        {
            var discountCode = await GetDiscountCodeByCodeAsync(code);
            if (discountCode == null)
                return false;

            var validationResult = ValidateDiscountCode(discountCode);
            return validationResult.IsValid;
        }

        public async Task<MemberShipDiscountCode?> UpdateDiscountCodeAsync(int id, MemberShipDiscountCode updatedDiscountCode)
        {
            var existingCode = await _context.MemberShipDiscountCodes.FindAsync(id);
            if (existingCode == null)
                return null;

            // Check if new code already exists (if code is being changed)
            if (existingCode.Code != updatedDiscountCode.Code)
            {
                var codeExists = await _context.MemberShipDiscountCodes
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
            var discountCode = await _context.MemberShipDiscountCodes.FindAsync(id);
            if (discountCode == null)
                return false;

            _context.MemberShipDiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DiscountValidationResult ValidateDiscountCode(MemberShipDiscountCode discountCode)
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



        public async Task<decimal> ApplyMembershipDiscountAsync(string code, int? MembershipId)
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
                var Membership = await _context.MemberShips.Include(c => c.MembershipCard).FirstOrDefaultAsync(o => o.Id == MembershipId && o.ApplyDiscount == false);
                if (Membership == null || Membership.Status == OrderStatus.Paid)
                    throw new ArgumentException("Membership Not found or paid or Applied already");

                var discountAmount = (Membership.MembershipCard.Price * discountCode.Percentage) / 100m;
                Membership.DiscountAmount = Membership.MembershipCard.Price - discountAmount;
                Membership.ApplyDiscount = true;
                Membership.MemberShipDiscountCodeId = discountCode.Id;

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
