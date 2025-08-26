using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class DiscountCode
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        public int Percentage { get; set; }
        
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; } = 0;
        public bool IsActive { get; set; }


        public DateTime ExpiryDate { get; set; }
        
        // Navigation properties
        public virtual User? User { get; set; }
        public int?  MemberShipId { get; set; }
        public virtual MemberShip? MemberShip { get; set; }
    }
}
