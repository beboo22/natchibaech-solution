using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Order
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public int? DiscountCode { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string BillingFirstName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string BillingLastName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string IdNumber { get; set; } = string.Empty;
        public bool ApplyDiscount { get; set; } = false;

        // Navigation properties

        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
