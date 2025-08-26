using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        public UserCategory Category { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
        public virtual int? MembershipId { get; set; }
        public virtual MemberShip? Membership { get; set; }
    }
}
