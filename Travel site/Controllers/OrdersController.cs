using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;


        public OrdersController(IOrderService orderService, IDiscountService discountService, IProductService productService)
        {
            _orderService = orderService;
            _discountService = discountService;
            _productService = productService;
        }

        /// <summary>
        /// Get all orders (User sees only their orders, Admin sees all)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] int? userId = null, [FromQuery] bool isAdmin = false)
        {
            try
            {
                IEnumerable<Order> orders;

                if (isAdmin)
                {
                    orders = await _orderService.GetAllOrdersAsync();
                }
                else if (userId.HasValue)
                {
                    orders = await _orderService.GetUserOrdersAsync(userId.Value);
                }
                else
                {
                    return BadRequest(new { message = "UserId is required for non-admin users" });
                }

                var orderDtos = orders.Select(MapToOrderDto);
                if (!orderDtos.Any())
                    return NotFound(new { message = "No orders found" });
                // use apiresponse to return the list of orders 
                return Ok(new ApiResponse<IEnumerable<OrderDto>>(200, orderDtos, "Orders retrieved successfully"));
                //return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Get order details by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                var orderDto = MapToOrderDto(order);
                // use apiresponse to return the order details
                return Ok(new ApiResponse<OrderDto>(200, orderDto, "Order retrieved successfully"));
                //return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new order from cart
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);




                var order = new Order
                {
                    UserId = createOrderDto.UserId,
                    BillingFirstName = createOrderDto.BillingFirstName,
                    BillingLastName = createOrderDto.BillingLastName,
                    Country = createOrderDto.Country,
                    IdNumber = createOrderDto.IdNumber,
                    Status = OrderStatus.Pending,

                };
                var totalprice = 0m;
                var orderitems = new List<OrderItem>();
                foreach (var item in createOrderDto.OrderItems)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        totalprice += product.UnitPrice * item.PersonNumber;
                        orderitems.Add(new OrderItem
                        {
                            ProductId = product.Id,
                            UnitPrice = product.UnitPrice * item.PersonNumber,
                            PersonNumber = item.PersonNumber,
                        });
                    }
                }
                order.OrderItems = orderitems;
                order.TotalAmount = totalprice;

                var createdOrder = await _orderService.CreateOrderAsync(order);

                var orderDto = MapToOrderDto(createdOrder!);
                // use apiresponse to return the created order
                return Ok(new ApiResponse<OrderDto>(201, orderDto, "Order created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        /// <summary>
        /// Update order status (Admin only) - Pending → Approved → Paid
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto.Status);
                if (updatedOrder == null)
                    return NotFound(new { message = "Order not found" });

                var orderDto = MapToOrderDto(updatedOrder);
                // use apiresponse to return the updated order
                return Ok(new ApiResponse<OrderDto>(200, orderDto, "Order status updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order status", error = ex.Message });
            }
        }

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.FullName ?? string.Empty,
                Status = order.Status,
                DiscountCode = order.DiscountCode,
                TotalAmount = order.TotalAmount,
                FinalAmount = order.FinalAmount,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                BillingFirstName = order.BillingFirstName,
                BillingLastName = order.BillingLastName,
                Country = order.Country,
                IdNumber = order.IdNumber,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? string.Empty,
                    PersonNumber = oi.PersonNumber,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.UnitPrice * oi.PersonNumber
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}
