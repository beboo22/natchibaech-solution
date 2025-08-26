using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Ticket
    {
        public int Id { get; set; }
        
        public int? OrderItemId { get; set; }
        public int? MemberShipId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string MemberName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string MembershipNumber { get; set; } = string.Empty;
        
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiryDate { get; set; }
        
        [MaxLength(200)]
        public string QRCode { get; set; } = string.Empty;
        
        public TicketDelivery DeliveryMethod { get; set; } = TicketDelivery.Email;
        
        public DateTime? SentAt { get; set; }
        
        // Navigation properties
        public virtual OrderItem? OrderItem { get; set; } = null!;
        public virtual MemberShip? MemberShip { get; set; } = null!;
    }
}
