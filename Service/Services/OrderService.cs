using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Transactions)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate that all products exist and get their current prices
                var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (products.Count != productIds.Count)
                {
                    throw new ArgumentException("One or more products not found");
                }

                // Calculate totals and set current prices
                decimal totalAmount = 0;
                foreach (var orderItem in order.OrderItems)
                {
                    var product = products.First(p => p.Id == orderItem.ProductId);
                    orderItem.UnitPrice = product.UnitPrice;
                    totalAmount += orderItem.UnitPrice * orderItem.PersonNumber;
                }

                order.TotalAmount = totalAmount;
                order.FinalAmount = totalAmount; // Will be updated if discount is applied
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                
                // Load the created order with all related data
                return await GetOrderByIdAsync(order.Id) ?? order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return null;

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return await GetOrderByIdAsync(id);
        }

        public async Task<Order?> ApplyDiscountToOrderAsync(int orderId, decimal discountAmount)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            order.FinalAmount = Math.Max(0, order.TotalAmount - discountAmount);
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return await GetOrderByIdAsync(orderId);
        }
    }
}
