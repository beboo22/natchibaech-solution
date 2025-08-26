using Domain.Entity;

namespace TicketingSystem.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountCode>> GetAllDiscountCodesAsync();
        Task<DiscountCode?> GetDiscountCodeByCodeAsync(string code);
        Task<DiscountCode> CreateDiscountCodeAsync(DiscountCode discountCode);
        Task<decimal> ApplyDiscountAsync(string code, int OrderId);
        Task<bool> ValidateDiscountCodeAsync(string code);
        Task<DiscountCode?> UpdateDiscountCodeAsync(int id, DiscountCode updatedDiscountCode);
        Task<bool> DeleteDiscountCodeAsync(int id);
    }
}
