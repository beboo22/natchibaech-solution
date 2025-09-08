using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        //private readonly IProductService _productService;
        private readonly IMembershipService _membershipService;
        private readonly ITicketService _ticketService;
        private readonly IOrderDiscountService _discountService;

        public AdminController(
            IUserService userService,
            IOrderService orderService,
            //IProductService productService,
            IMembershipService membershipService,
            ITicketService ticketService,
            IOrderDiscountService discountService)
        {
            _userService = userService;
            _orderService = orderService;
            //_productService = productService;
            _membershipService = membershipService;
            _ticketService = ticketService;
            _discountService = discountService;
        }

        /// <summary>
        /// Get comprehensive dashboard statistics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var orders = await _orderService.GetAllOrdersAsync();
                //var products = await _productService.GetAllProductsAsync();
                var membershipStats = await _membershipService.GetMembershipStatsAsync();
                var discountCodes = await _discountService.GetAllDiscountCodesAsync();

                var totalRevenue = orders
                    .Where(o => o.Status == OrderStatus.Paid)
                    .Sum(o => o.FinalAmount);

                var recentOrders = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .ToList();

                var ordersByStatus = orders
                    .GroupBy(o => o.Status)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                var res = new ApiResponse<object>(200, new
                {
                    Overview = new
                    {
                        TotalUsers = users.Count(),
                        TotalOrders = orders.Count(),
                        //TotalProducts = products.Count(),
                        TotalRevenue = totalRevenue,
                        PaidOrders = orders.Count(o => o.Status == OrderStatus.Paid),
                        PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending)
                    },
                    OrderStats = new
                    {
                        OrdersByStatus = ordersByStatus,
                        RecentOrders = recentOrders.Select(o => new
                        {
                            o.Id,
                            o.UserId,
                            UserName = o.User?.FullName ?? "Unknown",
                            o.Status,
                            o.TotalAmount,
                            o.FinalAmount,
                            o.CreatedAt
                        })
                    },
                    MembershipStats = membershipStats,
                    DiscountStats = new
                    {
                        TotalDiscountCodes = discountCodes.Count(),
                        ActiveDiscountCodes = discountCodes.Count(dc => dc.ExpiryDate > DateTime.UtcNow && dc.CurrentUsage < dc.MaxUsage),
                        ExpiredDiscountCodes = discountCodes.Count(dc => dc.ExpiryDate <= DateTime.UtcNow),
                        FullyUsedDiscountCodes = discountCodes.Count(dc => dc.CurrentUsage >= dc.MaxUsage)
                    }
                });
                Console.WriteLine(res);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system health status
        /// </summary>
        [HttpGet("health")]
        public ActionResult<object> GetSystemHealth()
        {
            try
            {
                return Ok(new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Services = new
                    {
                        Database = "Connected",
                        PaymentGateway = "Available",
                        EmailService = "Available",
                        QRCodeService = "Available"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "System health check failed", error = ex.Message });
            }
        }
    }


    public class DashboardResponseDto
    {
        public OverviewDto Overview { get; set; }
        public OrderStatsDto OrderStats { get; set; }
        public object MembershipStats { get; set; } // replace with actual type
        public DiscountStatsDto DiscountStats { get; set; }
    }

    public class OverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PaidOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class OrderStatsDto
    {
        public Dictionary<string, int> OrdersByStatus { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; }
    }

    public class RecentOrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DiscountStatsDto
    {
        public int TotalDiscountCodes { get; set; }
        public int ActiveDiscountCodes { get; set; }
        public int ExpiredDiscountCodes { get; set; }
        public int FullyUsedDiscountCodes { get; set; }
    }

}
