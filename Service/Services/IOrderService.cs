using Domain.Entity;

namespace TicketingSystem.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status);
        Task<Order?> ApplyDiscountToOrderAsync(int orderId, decimal discountAmount);
    }
}
