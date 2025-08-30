using Domain.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travelsite.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int? DiscountCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string BillingFUllName { get; set; } = string.Empty;
        //public string BillingLastName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class CreateOrderDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string BillingFUllName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string IdNumber { get; set; } = string.Empty;
        
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int PersonNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreateOrderItemDto
    {
        public ServiceCategory ServiceCategory { get; set; }

        public DateTime BookingDate { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal UnitPrice { get; set; }




        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Person number must be at least 1")]
        public int PersonNumber { get; set; }
    }
}
