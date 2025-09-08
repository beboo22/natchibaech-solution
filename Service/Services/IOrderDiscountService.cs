using Domain.Entity;

namespace TicketingSystem.Services
{
    public interface IOrderDiscountService
    {
        Task<IEnumerable<OrderDiscountCode>> GetAllDiscountCodesAsync();
        Task<OrderDiscountCode?> GetDiscountCodeByCodeAsync(string code);
        Task<OrderDiscountCode> CreateDiscountCodeAsync(OrderDiscountCode discountCode);
        Task<decimal> ApplyOrderDiscountAsync(string code, int? OrderId);
        Task<bool> ValidateDiscountCodeAsync(string code);
        Task<OrderDiscountCode?> UpdateDiscountCodeAsync(int id, OrderDiscountCode updatedDiscountCode);
        Task<bool> DeleteDiscountCodeAsync(int id);
    }
}
